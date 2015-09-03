using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LightningQueues.Exceptions;
using LightningQueues.Model;
using LightningQueues.Protocol;
using LightningQueues.Utils;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class GlobalActions : AbstractActions
    {
	    private readonly QueueManagerConfiguration configuration;

        public GlobalActions(JET_INSTANCE instance, ColumnsInformation columnsInformation, string database, Guid instanceId, QueueManagerConfiguration configuration)
			: base(instance, columnsInformation, database, instanceId)
        {
            this.configuration = configuration;
        }

        public void CreateQueueIfDoesNotExists(string queueName)
        {
            var enumerator = queues.GetEnumerator(new StringValueIndex(queueName));
            if (enumerator.MoveNext())
                return;

            new QueueSchemaCreator(session, dbid, queueName).Create();
            queues.Insert(() =>
            {
                queues.ForColumnType<StringColumn>().Set("name", queueName);
                queues.ForColumnType<DateTimeColumn>().Set("created_at", DateTime.Now);
            });
        }

        public void RegisterRecoveryInformation(Guid transactionId, byte[] information)
        {
            recovery.Insert(() =>
            {
                recovery.ForColumnType<GuidColumn>().Set("tx_id", transactionId);
                recovery.ForColumnType<BytesColumn>().Set("recovery_info", information);
            });
        }

        public void DeleteRecoveryInformation(Guid transactionId)
        {
            var enumerator = recovery.GetEnumerator(new GuidIndex(transactionId));
            if(enumerator.MoveNext())
                recovery.Delete();
        }

        public IEnumerable<byte[]> GetRecoveryInformation()
        {
            var enumerator = recovery.GetEnumerator();
            while (enumerator.MoveNext())
                yield return recovery.ForColumnType<BytesColumn>().Get("recovery_info");
        }

        public void RegisterUpdateToReverse(Guid txId, MessageBookmark bookmark, MessageStatus statusToRestore, string subQueue)
        {
            var actualBookmark = bookmark.Bookmark.Take(bookmark.Size).ToArray();
            var enumerator = txs.GetEnumerator(new BookmarkIndex(bookmark.Size, actualBookmark));

			if(enumerator.MoveNext())
			{
                txs.Delete();
			}

            txs.Insert(() =>
            {
                txs.ForColumnType<GuidColumn>().Set("tx_id", txId);
                txs.ForColumnType<IntColumn>().Set("bookmark_size", bookmark.Size);
                txs.ForColumnType<BytesColumn>().Set("bookmark_data", actualBookmark);
                txs.ForColumnType<IntColumn>().Set("value_to_restore", (int)statusToRestore);
                txs.ForColumnType<StringColumn>().Set("queue", bookmark.QueueName);
                txs.ForColumnType<StringColumn>().Set("subqueue", subQueue);
            });
        }

        public void RemoveReversalsMoveCompletedMessagesAndFinishSubQueueMove(Guid transactionId)
        {
            var enumerator = txs.GetEnumerator(new GuidIndex(transactionId, "by_tx_id"));
            while(enumerator.MoveNext())
            {
                var queue = txs.ForColumnType<StringColumn>().Get("queue");

                var actions = GetQueue(queue);

                var bookmark = new MessageBookmark
                {
                    Bookmark = txs.ForColumnType<BytesColumn>().Get("bookmark_data"),
                    QueueName = queue,
                    Size = txs.ForColumnType<IntColumn>().Get("bookmark_size")
                };

                switch (actions.GetMessageStatus(bookmark))
                {
                    case MessageStatus.SubqueueChanged:
                    case MessageStatus.EnqueueWait:
                        actions.SetMessageStatus(bookmark, MessageStatus.ReadyToDeliver);
                        break;
                    default:
                        if (configuration.EnableProcessedMessageHistory)
                            actions.MoveToHistory(bookmark);
                        else
                            actions.Delete(bookmark);
                        break;
                }

                txs.Delete();
            }
        }

        public Guid RegisterToSend(Endpoint destination, string queue, string subQueue, MessagePayload payload, Guid transactionId)
        {
			var msgId = GuidCombGenerator.Generate();
            var bookmark = outgoing.Insert(() =>
            {
                outgoing.ForColumnType<GuidColumn>().Set("msg_id", msgId);
                outgoing.ForColumnType<GuidColumn>().Set("tx_id", transactionId);
                outgoing.ForColumnType<StringColumn>().Set("address", destination.Host);
                outgoing.ForColumnType<IntColumn>().Set("port", destination.Port);
                outgoing.ForColumnType<DateTimeColumn>().Set("time_to_send", DateTime.Now);
                outgoing.ForColumnType<DateTimeColumn>().Set("sent_at", DateTime.Now);
                outgoing.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.NotReady);
                outgoing.ForColumnType<StringColumn>().Set("queue", queue);
                outgoing.ForColumnType<StringColumn>().Set("subqueue", subQueue);
                outgoing.ForColumnType<StringColumn>().Set("headers", payload.Headers.ToQueryString());
                outgoing.ForColumnType<BytesColumn>().Set("data", payload.Data);
                outgoing.ForColumnType<IntColumn>().Set("number_of_retries", 1);
                outgoing.ForColumnType<IntColumn>().Set("size_of_data", payload.Data.Length);
                if(payload.DeliverBy.HasValue)
                    outgoing.ForColumnType<DateTimeColumn>().Set("deliver_by", payload.DeliverBy.Value);
                if(payload.MaxAttempts.HasValue)
                    outgoing.ForColumnType<IntColumn>().Set("max_attempts", payload.MaxAttempts.Value);
            });
            outgoing.MoveTo(bookmark);
            logger.Debug("Created output message '{0}' for 'lq.tcp://{1}:{2}/{3}/{4}' as NotReady",
                msgId,
                destination.Host,
                destination.Port,
                queue,
                subQueue
                );
            return msgId;
        }

        public void MarkAsReadyToSend(Guid transactionId)
        {
            var enumerator = outgoing.GetEnumerator(new GuidIndex(transactionId, "by_tx_id"));
            while(enumerator.MoveNext())
            {
                outgoing.Update(() => outgoing.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.Ready));
                var id = outgoing.ForColumnType<GuidColumn>().Get("msg_id");
                logger.Debug("Marking output message {0} as Ready", id);
            }
        }

        public void DeleteMessageToSend(Guid transactionId)
        {
            var enumerator = outgoing.GetEnumerator(new GuidIndex(transactionId, "by_tx_id"));
        	while(enumerator.MoveNext())
        	{
        	    var id = outgoing.ForColumnType<GuidColumn>().Get("msg_id");
        	    logger.Debug("Deleting output message {0}", id);
                outgoing.Delete();
            }
        }

        public void MarkAllOutgoingInFlightMessagesAsReadyToSend()
        {
            var enumerator = outgoing.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var status = (OutgoingMessageStatus) outgoing.ForColumnType<IntColumn>().Get("send_status");
                if (status != OutgoingMessageStatus.InFlight)
                    continue;

                outgoing.Update(() => outgoing.ForColumnType<IntColumn>().Set("send_status", (int)OutgoingMessageStatus.Ready));
            }
        }

        public void MarkAllProcessedMessagesWithTransactionsNotRegisterForRecoveryAsReadyToDeliver()
        {
            var txsWithRecovery = new HashSet<Guid>();
            var enumerator = recovery.GetEnumerator();
            while (enumerator.MoveNext())
            {
                txsWithRecovery.Add(recovery.ForColumnType<GuidColumn>().Get("tx_id"));
            }

            var txsWithoutRecovery = new HashSet<Guid>();
            enumerator = txs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                txsWithoutRecovery.Add(txs.ForColumnType<GuidColumn>().Get("tx_id"));
            }

            foreach (var txId in txsWithoutRecovery)
            {
                if (txsWithRecovery.Contains(txId))
                    continue;
                ReverseAllFrom(txId);
            }
        }

        public void ReverseAllFrom(Guid transactionId)
        {
            var enumerator = txs.GetEnumerator(new GuidIndex(transactionId, "by_tx_id"));

            while(enumerator.MoveNext())
            {
                try
                {
                    var oldStatus = (MessageStatus)txs.ForColumnType<IntColumn>().Get("value_to_restore");
                    var queue = txs.ForColumnType<StringColumn>().Get("queue");
                    var subqueue = txs.ForColumnType<StringColumn>().Get("subqueue");

                    var bookmark = new MessageBookmark
                    {
                        QueueName = queue,
                        Bookmark = txs.ForColumnType<BytesColumn>().Get("bookmark_data"),
                        Size = txs.ForColumnType<IntColumn>().Get("bookmark_size")
                    };
                    var actions = GetQueue(queue);
                    var newStatus = actions.GetMessageStatus(bookmark);
                    switch (newStatus)
                    {
                        case MessageStatus.SubqueueChanged:
                            actions.SetMessageStatus(bookmark, MessageStatus.ReadyToDeliver, subqueue);
                            break;
                        case MessageStatus.EnqueueWait:
                            actions.Delete(bookmark);
                            break;
                        default:
                            actions.SetMessageStatus(bookmark, oldStatus);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to reverse a transaction", ex);
                }
            }
        }

        public string[] GetAllQueuesNames()
        {
            var names = new List<string>();
            var enumerator = queues.GetEnumerator();
            while (enumerator.MoveNext())
            {
                names.Add(queues.ForColumnType<StringColumn>().Get("name"));
            }
            return names.ToArray();
        }

	    public MessageBookmark GetSentMessageBookmarkAtPosition(int positionFromNewestSentMessage)
	    {
	        var enumerator = outgoingHistory.GetEnumerator(new PositionalIndexFromLast(positionFromNewestSentMessage));

	        return !enumerator.MoveNext() ? null : enumerator.Current;
	    }

	    public IEnumerable<PersistentMessageToSend> GetSentMessages(int? batchSize = null)
	    {
	        var enumerator = outgoingHistory.GetEnumerator();

            int count = 0;
            while (enumerator.MoveNext() && count++ != batchSize)
            {
                var address = outgoingHistory.ForColumnType<StringColumn>().Get("address");
                var port = outgoingHistory.ForColumnType<IntColumn>().Get("port");

                var bookmark = enumerator.Current;

                yield return new PersistentMessageToSend
                {
                    Id = new MessageId
                    {
                        SourceInstanceId = instanceId,
                        MessageIdentifier = outgoingHistory.ForColumnType<GuidColumn>().Get("msg_id"),
                    },
                    OutgoingStatus = (OutgoingMessageStatus) outgoingHistory.ForColumnType<IntColumn>().Get("send_status"),
                    Endpoint = new Endpoint(address, port),
                    Queue = outgoingHistory.ForColumnType<StringColumn>().Get("queue"),
                    SubQueue = outgoingHistory.ForColumnType<StringColumn>().Get("subqueue"),
                    SentAt = outgoingHistory.ForColumnType<DateTimeColumn>().Get("sent_at"),
                    Data = outgoingHistory.ForColumnType<BytesColumn>().Get("data"),
                    Headers = HttpUtility.ParseQueryString(outgoingHistory.ForColumnType<StringColumn>().Get("headers")),
                    Bookmark = bookmark
                };
            }
        }

        public void DeleteMessageToSendHistoric(MessageBookmark bookmark)
        {
            outgoingHistory.MoveTo(bookmark);
            outgoingHistory.Delete();
        }

    	public int GetNumberOfMessages(string queueName)
    	{
    	    var enumerator = queues.GetEnumerator(new StringValueIndex("pk", queueName));

			if (!enumerator.MoveNext())
                throw new QueueDoesNotExistsException(queueName);

    	    return queues.ForColumnType<IntColumn>().InterlockedRead("number_of_messages");
    	}

		public IEnumerable<MessageId> GetAlreadyReceivedMessageIds()
		{
		    var enumerator = recveivedMsgs.GetEnumerator();
			while(enumerator.MoveNext())
			{
			    yield return recveivedMsgs.GetMessageId();
			}
		}

		public void MarkReceived(MessageId id)
		{
		    recveivedMsgs.Insert(() =>
		    {
                recveivedMsgs.ForColumnType<GuidColumn>().Set("instance_id", id.SourceInstanceId);
                recveivedMsgs.ForColumnType<GuidColumn>().Set("msg_id", id.MessageIdentifier);
		    });
		}

		public IEnumerable<MessageId> DeleteOldestReceivedMessageIds(int numberOfItemsToKeep, int numberOfItemsToDelete)
		{
		    var enumerator = recveivedMsgs.GetEnumerator(new PositionalIndexFromLast(numberOfItemsToKeep + 1), true);
			while(enumerator.MoveNext() && numberOfItemsToDelete-- > 0)
			{
			    var id = recveivedMsgs.GetMessageId();
                recveivedMsgs.Delete();
			    yield return id;
			}
		}

        public PersistentMessageToSend GetSentMessageById(Guid id)
        {
            var enumerator = outgoingHistory.GetEnumerator(new GuidIndex(id));

            while (enumerator.MoveNext())
            {
                var address = outgoingHistory.ForColumnType<StringColumn>().Get("address");
                var port = outgoingHistory.ForColumnType<IntColumn>().Get("port");

                var bookmark = enumerator.Current;

                return new PersistentMessageToSend
                {
                    Id = new MessageId
                    {
                        SourceInstanceId = instanceId,
                        MessageIdentifier = outgoingHistory.ForColumnType<GuidColumn>().Get("msg_id"),
                    },
                    OutgoingStatus = (OutgoingMessageStatus)outgoingHistory.ForColumnType<IntColumn>().Get("send_status"),
                    Endpoint = new Endpoint(address, port),
                    Queue = outgoingHistory.ForColumnType<StringColumn>().Get("queue"),
                    SubQueue = outgoingHistory.ForColumnType<StringColumn>().Get("subqueue"),
                    SentAt = outgoingHistory.ForColumnType<DateTimeColumn>().Get("sent_at"),
                    Data = outgoingHistory.ForColumnType<BytesColumn>().Get("data"),
                    Headers = HttpUtility.ParseQueryString(outgoingHistory.ForColumnType<StringColumn>().Get("headers")),
                    Bookmark = bookmark
                };
            }
            return null;
        }
    }
}
