using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LightningQueues.Logging;
using LightningQueues.Serialization;
using LightningQueues.Storage;

namespace LightningQueues.Net.Protocol.V1
{
    public class ReceivingProtocol : IReceivingProtocol
    {
        readonly IMessageStore _store;
        readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        public ReceivingProtocol(IMessageStore store, ILogger logger) : this(store, logger, TaskPoolScheduler.Default)
        {
        }

        public ReceivingProtocol(IMessageStore store, ILogger logger, IScheduler scheduler)
        {
            _store = store;
            _logger = logger;
            _scheduler = scheduler;
        }
        

        public IObservable<Message> ReceiveStream(IObservable<Stream> streams, string remoteEndpoint)
        {
            return receiveStream(streams, remoteEndpoint).Timeout(TimeSpan.FromSeconds(5), _scheduler);
        }

        private IObservable<Message> receiveStream(IObservable<Stream> streams, string remoteEndpoint)
        {
            return from stream in streams.Do(x => _logger.DebugFormat("Starting to read stream from {0}", remoteEndpoint))
                   from length in LengthChunk(stream).Do(x => _logger.DebugFormat("Reading in {0} messages from {1}", x, remoteEndpoint))
                   from messages in MessagesChunk(stream, length).DoAsync(x => StoreMessages(stream, x)).Do(x => _logger.DebugFormat("Stored messages from {0}", remoteEndpoint))
                   from _r in SendReceived(stream).Do(x => _logger.DebugFormat("Sending received bytes to {0}", remoteEndpoint))
                   from _a in ReceiveAcknowledgement(stream, messages).Do(x => _logger.DebugFormat("Received acknowledgement from {0}", remoteEndpoint))
                   from message in messages
                   select message; 
        }

        public IObservable<int> LengthChunk(Stream stream)
        {
            return Observable.FromAsync(() => stream.ReadBytesAsync(sizeof(int)))
                .Select(x => BitConverter.ToInt32(x, 0))
                .Catch((Exception ex) => sendSerializationError<int>(stream, ex))
                .Do(x => _logger.DebugFormat("Read in length value of {0}", x))
                .Where(x => x > 0);
        }

        public IObservable<Message[]> MessagesChunk(Stream stream, int length)
        {
            return Observable.FromAsync(() => stream.ReadBytesAsync(length))
                .Select(x => x.ToMessages()).Do(x => _logger.Debug("Successfully read messages"))
                .Catch((Exception ex) => sendSerializationError<Message[]>(stream, ex));
        }

        private IObservable<T> sendSerializationError<T>(Stream stream, Exception ex)
        {
            _logger.Error("Error deserializing messages", ex);
            return from _ in Observable.FromAsync(() => SendBuffer(stream, Constants.SerializationFailureBuffer))
                   from empty in Observable.Empty<T>()
                   select empty;
        }

        private async Task SendBuffer(Stream stream, byte[] buffer)
        {
            await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        public async Task StoreMessages(Stream stream, params Message[] messages)
        {
            try
            {
                _store.StoreIncomingMessages(messages);
                _logger.Debug("Finished storing messages");
            }
            catch(QueueDoesNotExistException ex)
            {
                _logger.Error("Received a message for a queue that doesn't exist", ex);
                await SendBuffer(stream, Constants.QueueDoesNotExistBuffer);
                throw;
            }
            catch(Exception ex)
            {
                _logger.Error("Error persisting messages in receiver", ex);
                await SendBuffer(stream, Constants.ProcessingFailureBuffer);
                throw;
            }
        }

        public IObservable<Unit> SendReceived(Stream stream)
        {
            return Observable.FromAsync(() => SendBuffer(stream, Constants.ReceivedBuffer));
        }

        public IObservable<Unit> ReceiveAcknowledgement(Stream stream, Message[] messages)
        {
            return Observable.FromAsync(() => stream.ReadExpectedBuffer(Constants.AcknowledgedBuffer))
                .Do(acknowledged =>
                {
                    if (!acknowledged)
                    {
                        _store.DeleteIncomingMessages(messages);
                    }
                    _logger.Debug("Acknowledgement received was " + acknowledged);
                })
                .Where(x => x)
                .Select(x => Unit.Default);
        }
    }
}