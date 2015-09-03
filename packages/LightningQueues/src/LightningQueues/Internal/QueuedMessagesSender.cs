using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using LightningQueues.Exceptions;
using LightningQueues.Logging;
using LightningQueues.Model;
using LightningQueues.Protocol;
using LightningQueues.Storage;
using System;
using System.Collections.Generic;
using LightningQueues.Utils;
using Microsoft.Isam.Esent;

namespace LightningQueues.Internal
{
    public class QueuedMessagesSender
    {
        private readonly QueueStorage _queueStorage;
        private readonly ILogger _logger;
        private readonly SendingChoke _choke;
        private volatile bool _continueSending = true;

        public QueuedMessagesSender(QueueStorage queueStorage, SendingChoke choke, ILogger logger)
        {
        	_queueStorage = queueStorage;
            _logger = logger;
            _choke = choke;
        }

        public void Send()
        {
            var allEndpoints = new ThreadSafeSet<Endpoint>();
            while (_continueSending)
            {
                if(!_choke.ShouldBeginSend())
                    continue;

                var endpoints = gatherEndpoints(allEndpoints.All()).ToArray();
                if (!endpoints.Any())
                {
                    _choke.NoMessagesToSend();
                    continue;
                }
                allEndpoints.Add(endpoints);

                endpoints.Each(endpoint =>
                {
                    var messages = gatherMessagesToSend(endpoint);
                    if (!messages.Any())
                    {
                        allEndpoints.Remove(endpoint);
                        return;
                    }

                    _choke.StartSend();

                    sendMessages(endpoint, messages)
                        .ContinueWith(x => allEndpoints.Remove(endpoint));
                });
            }
        }

        private async Task sendMessages(Endpoint destination, PersistentMessage[] messages)
        {
            var sender = createSender(destination, messages);
            MessageBookmark[] sendHistoryBookmarks = null;
            sender.Success = () => sendHistoryBookmarks = success(messages);
            try
            {
                await sender.Send().ConfigureAwait(false);
                _logger.MessagesSent(messages, destination);
            }
            catch (FailedToConnectException ex)
            {
                _logger.FailedToSend(destination, "Failed to connect", ex);
                failedToConnect(messages);
            }
            catch (QueueDoesNotExistsException)
            {
                _logger.FailedToSend(destination, "Queue doesn't exist");
                failedToSend(messages, true);
            }
            catch (RevertSendException)
            {
                _logger.FailedToSend(destination, "Revert was received");
                revert(sendHistoryBookmarks, messages);
            }
            catch (TimeoutException)
            {
                try
                {
                    _logger.FailedToSend(destination, "Timed out");
                    failedToSend(messages);
                }
                catch (EsentException)
                {
                    // This will occur if the task completed as the TimeoutException was thrown, and 
                    // the message was moved to history.  Swallow it to prevent unobserved task exceptions.
                }
            }
            catch (Exception ex)
            {
                _logger.FailedToSend(destination, "Exception was thrown", ex);
                failedToSend(messages);
            }
        }

        private Sender createSender(Endpoint destination, PersistentMessage[] messages)
        {
            return new Sender
            {
                Connected = () => _choke.SuccessfullyConnected(),
                Destination = destination,
                Messages = messages,
            };
        }

        private IEnumerable<Endpoint> gatherEndpoints(IEnumerable<Endpoint> currentlySending)
        {
            return _queueStorage.Send(actions => actions.GetEndpointsToSend(currentlySending, _choke.AvailableSendingCount));
        }

        private PersistentMessage[] gatherMessagesToSend(Endpoint endpoint)
        {
            return _queueStorage.Send(actions => actions.GetMessagesToSendAndMarkThemAsInFlight(100, 1024 * 1024, endpoint));
        }

        private void failedToConnect(IEnumerable<PersistentMessage> messages)
        {
            _choke.FailedToConnect();
            failedToSend(messages);
        }

        private void failedToSend(IEnumerable<PersistentMessage> messages, bool queueDoesntExist = false)
        {
            try
            {
                _queueStorage.Send(actions =>
                {
                    foreach (var message in messages)
                    {
                        actions.MarkOutgoingMessageAsFailedTransmission(message.Bookmark, queueDoesntExist);
                    }
                });
            }
            finally
            {
                _choke.FinishedSend();
            }
        }

        private void revert(MessageBookmark[] sendHistoryBookmarks, IEnumerable<PersistentMessage> messages)
        {
            _queueStorage.Send(actions => actions.RevertBackToSend(sendHistoryBookmarks));
            failedToSend(messages);
        }

        private MessageBookmark[] success(IEnumerable<PersistentMessage> messages)
        {
            try
            {
                var newBookmarks = _queueStorage.Send(actions => 
                    messages.Select(message => actions.MarkOutgoingMessageAsSuccessfullySent(message.Bookmark)).ToArray());
                return newBookmarks;
            }
            finally
            {
                _choke.FinishedSend();
            }
        }

        public void Stop()
        {
            _continueSending = false;
            _choke.StopSending();
        }
    }
}