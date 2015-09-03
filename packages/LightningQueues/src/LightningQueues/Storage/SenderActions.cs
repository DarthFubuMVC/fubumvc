using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LightningQueues.Model;
using LightningQueues.Protocol;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class SenderActions : AbstractActions
    {
        private readonly QueueManagerConfiguration configuration;

		public SenderActions(JET_INSTANCE instance, ColumnsInformation columnsInformation, string database, Guid instanceId, QueueManagerConfiguration configuration)
            : base(instance, columnsInformation, database, instanceId)
		{
		    this.configuration = configuration;
		}

        public IEnumerable<Endpoint> GetEndpointsToSend(IEnumerable<Endpoint> currentlySending, int numberOfEndpoints)
        {
            var endpoints = new HashSet<Endpoint>();
            var enumerator = outgoing.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var time = outgoing.ForColumnType<DateTimeColumn>().Get("time_to_send");
                if (time > DateTime.Now)
                    continue;

                var endpoint = new Endpoint( outgoing.ForColumnType<StringColumn>().Get("address"),
                    outgoing.ForColumnType<IntColumn>().Get("port"));
                if (!endpoints.Contains(endpoint) && !currentlySending.Contains(endpoint))
                {
                    endpoints.Add(endpoint);
                    if (endpoints.Count == numberOfEndpoints)
                        break;
                }
            }
            return endpoints;
        }

        public PersistentMessage[] GetMessagesToSendAndMarkThemAsInFlight(int maxNumberOfMessage, int maxSizeOfMessagesInTotal, Endpoint endpoint)
        {
            var enumerator = outgoing.GetEnumerator();

        	string queue = null;
            var messages = new List<PersistentMessage>();

            while (enumerator.MoveNext())
            {
                var msgId = outgoing.ForColumnType<GuidColumn>().Get("msg_id");
                var value = (OutgoingMessageStatus) outgoing.ForColumnType<IntColumn>().Get("send_status");
                var time = outgoing.ForColumnType<DateTimeColumn>().Get("time_to_send");

                logger.Debug("Scanning message {0} with status {1} to be sent at {2}", msgId, value, time);
                if (value != OutgoingMessageStatus.Ready)
                    continue;

                // Check if the message has expired, and move it to the outgoing history.
                var deliverByTime = outgoing.ForColumnType<DateTimeColumn>().GetOrDefault("deliver_by");
                if (deliverByTime.HasValue)
                {
                    if (deliverByTime < DateTime.Now)
                    {
                        logger.Info("Outgoing message {0} was not succesfully sent by its delivery time limit {1}", msgId, deliverByTime);
                        var numOfRetries = outgoing.ForColumnType<IntColumn>().Get("number_of_retries");
                        MoveFailedMessageToOutgoingHistory(numOfRetries, msgId);
                        continue;
                    }
                }

                var maxAttempts = outgoing.ForColumnType<IntColumn>().GetOrDefault("max_attempts");
                if (maxAttempts != null)
                {
                    var numOfRetries = outgoing.ForColumnType<IntColumn>().Get("number_of_retries");
                    if (numOfRetries > maxAttempts)
                    {
                        logger.Info("Outgoing message {0} has reached its max attempts of {1}", msgId, maxAttempts);
                        MoveFailedMessageToOutgoingHistory(numOfRetries, msgId);
                        continue;
                    }
                }

                if (time > DateTime.Now)
                    continue;

                var rowEndpoint = new Endpoint(
                    outgoing.ForColumnType<StringColumn>().Get("address"),
                    outgoing.ForColumnType<IntColumn>().Get("port"));

                if (endpoint.Equals(rowEndpoint) == false)
                    continue;

                var rowQueue = outgoing.ForColumnType<StringColumn>().Get("queue");

				if (queue == null) 
					queue = rowQueue;

				if(queue != rowQueue)
					continue;
                
                var bookmark = enumerator.Current;

                logger.Debug("Adding message {0} to returned messages", msgId);
            	messages.Add(new PersistentMessage
                {
                    Id = new MessageId
                    {
                        SourceInstanceId = instanceId,
                        MessageIdentifier = msgId
                    },
                    Headers = HttpUtility.ParseQueryString(outgoing.ForColumnType<StringColumn>().Get("headers")),
                    Queue = rowQueue,
                    SubQueue = outgoing.ForColumnType<StringColumn>().Get("subqueue"),
                    SentAt = outgoing.ForColumnType<DateTimeColumn>().Get("sent_at"),
                    Data = outgoing.ForColumnType<BytesColumn>().Get("data"),
                    Bookmark = bookmark
                });

                outgoing.Update(() => outgoing.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.InFlight));

                logger.Debug("Marking output message {0} as InFlight", msgId);

                if (maxNumberOfMessage < messages.Count)
                    break;
                if (maxSizeOfMessagesInTotal < messages.Sum(x => x.Data.Length))
                    break;
            }
            return messages.ToArray();
        }

        public void MarkOutgoingMessageAsFailedTransmission(MessageBookmark bookmark, bool queueDoesNotExistsInDestination)
        {
            outgoing.MoveTo(bookmark);
            var numOfRetries = outgoing.ForColumnType<IntColumn>().Get("number_of_retries");
            var msgId = outgoing.ForColumnType<GuidColumn>().Get("msg_id");

            if (numOfRetries < 100 && queueDoesNotExistsInDestination == false)
            {
                outgoing.Update(() =>
                {
                    outgoing.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.Ready);
                    outgoing.ForColumnType<DateTimeColumn>().Set("time_to_send", DateTime.Now.AddSeconds(numOfRetries * numOfRetries));
                    outgoing.ForColumnType<IntColumn>().Set("number_of_retries", numOfRetries + 1);
                    logger.Debug("Marking outgoing message {0} as failed with retries: {1}", msgId, numOfRetries);
                });
            }
            else
            {
                MoveFailedMessageToOutgoingHistory(numOfRetries, msgId);
            }
        }

        public MessageBookmark MarkOutgoingMessageAsSuccessfullySent(MessageBookmark bookmark)
        {
            outgoing.MoveTo(bookmark);
            var newBookmark = outgoingHistory.Insert(() =>
            {
                foreach (var column in outgoing.ColumnNames)
                {
                    var bytes = outgoing.ForColumnType<BytesColumn>().Get(column);
                    outgoingHistory.ForColumnType<BytesColumn>().Set(column, bytes);
                }
                outgoingHistory.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.Sent);
            });
            var msgId = outgoing.ForColumnType<GuidColumn>().Get("msg_id");
            outgoing.Delete();
            logger.Debug("Successfully sent output message {0}", msgId);
            return newBookmark;
        }

        public bool HasMessagesToSend()
        {
            var enumerator = outgoing.GetEnumerator();
            return enumerator.MoveNext();
		}

        public IEnumerable<PersistentMessageToSend> GetMessagesToSend()
        {
            var enumerator = outgoing.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var address = outgoing.ForColumnType<StringColumn>().Get("address");
                var port = outgoing.ForColumnType<IntColumn>().Get("port");

                var bookmark = enumerator.Current;

                yield return new PersistentMessageToSend
                {
                    Id = new MessageId
                    {
                        SourceInstanceId = instanceId,
						MessageIdentifier = outgoing.ForColumnType<GuidColumn>().Get("msg_id")
                    },
                    OutgoingStatus = (OutgoingMessageStatus)outgoing.ForColumnType<IntColumn>().Get("send_status"),
                    Endpoint = new Endpoint(address, port),
                    Queue = outgoing.ForColumnType<StringColumn>().Get("queue"),
                    SubQueue = outgoing.ForColumnType<StringColumn>().Get("subqueue"),
                    SentAt = outgoing.ForColumnType<DateTimeColumn>().Get("sent_at"),
                    Data = outgoing.ForColumnType<BytesColumn>().Get("data"),
                    Headers = HttpUtility.ParseQueryString(outgoing.ForColumnType<StringColumn>().Get("headers")),
                    Bookmark = bookmark
                };
            }
        }

        public void RevertBackToSend(MessageBookmark[] bookmarks)
        {
            foreach (var bookmark in bookmarks)
            {
                outgoingHistory.MoveTo(bookmark);
                var msgId = outgoingHistory.ForColumnType<GuidColumn>().Get("msg_id");

                outgoing.Insert(() =>
                {
                    foreach (var column in ColumnsInformation.OutgoingColumns.Keys)
                    {
                        var bytes = outgoingHistory.ForColumnType<BytesColumn>().Get(column);
                        outgoing.ForColumnType<BytesColumn>().Set(column, bytes);
                    }
                    outgoing.ForColumnType<IntColumn>().Set("send_status", (int) OutgoingMessageStatus.Ready);
                    var previousRetry = outgoingHistory.ForColumnType<IntColumn>().Get("number_of_retries");
                    outgoing.ForColumnType<IntColumn>().Set("number_of_retries", previousRetry + 1);

                    logger.Debug("Reverting output message {0} back to Ready mode", msgId);
                });
                outgoingHistory.Delete();
            }
        }

        private void MoveFailedMessageToOutgoingHistory(int numOfRetries, Guid msgId)
        {
            if (configuration.EnableOutgoingMessageHistory)
            {
                outgoingHistory.Insert(() =>
                {
                    foreach (var column in ColumnsInformation.OutgoingColumns.Keys)
                    {
                        outgoingHistory.ForColumnType<BytesColumn>().Set(column,
                            outgoing.ForColumnType<BytesColumn>().Get(column));
                    }
                    outgoingHistory.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.Failed);

                    logger.Debug("Marking outgoing message {0} as permenantly failed after {1} retries",
                        msgId, numOfRetries);

                });
            }
            outgoing.Delete();
        }

        public PersistentMessageToSend GetMessageToSendById(Guid id)
        {
            var enumerator = outgoing.GetEnumerator(new GuidIndex(id));

            while (enumerator.MoveNext())
            {
                var address = outgoing.ForColumnType<StringColumn>().Get("address");
                var port = outgoing.ForColumnType<IntColumn>().Get("port");

                var bookmark = enumerator.Current;

                return new PersistentMessageToSend
                {
                    Id = new MessageId
                    {
                        SourceInstanceId = instanceId,
                        MessageIdentifier = outgoing.ForColumnType<GuidColumn>().Get("msg_id")
                    },
                    OutgoingStatus = (OutgoingMessageStatus)outgoing.ForColumnType<IntColumn>().Get("send_status"),
                    Endpoint = new Endpoint(address, port),
                    Queue = outgoing.ForColumnType<StringColumn>().Get("queue"),
                    SubQueue = outgoing.ForColumnType<StringColumn>().Get("subqueue"),
                    SentAt = outgoing.ForColumnType<DateTimeColumn>().Get("sent_at"),
                    Data = outgoing.ForColumnType<BytesColumn>().Get("data"),
                    Headers = HttpUtility.ParseQueryString(outgoing.ForColumnType<StringColumn>().Get("headers")),
                    Bookmark = bookmark
                };
            }
            return null;
        }
    }
}