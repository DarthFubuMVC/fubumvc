using System;
using System.Collections.Generic;
using System.Linq;
using LightningQueues.Logging;
using LightningQueues.Model;
using LightningQueues.Protocol;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public class QueueActions : IDisposable
	{
		private readonly string _queueName;
		private string[] _subqueues;
		private readonly AbstractActions _actions;
		private readonly Action<int> _changeNumberOfMessages;
        private readonly ILogger _logger = LogManager.GetLogger<QueueActions>();
        private readonly EsentTable _messages;
        private readonly EsentTable _messageHistory;

		public QueueActions(Session session, JET_DBID dbid, string queueName, string[] subqueues, AbstractActions actions, Action<int> changeNumberOfMessages)
		{
			_queueName = queueName;
			_subqueues = subqueues;
			_actions = actions;
			_changeNumberOfMessages = changeNumberOfMessages;
		    var msgs = new Table(session, dbid, queueName, OpenTableGrbit.None);
			var msgsHistory = new Table(session, dbid, queueName + "_history", OpenTableGrbit.None);
            _messages = new EsentTable(session, msgs);
            _messageHistory = new EsentTable(session, msgsHistory);
		}

		public string[] Subqueues
		{
			get
			{
				return _subqueues;
			}
		}

		public MessageBookmark Enqueue(Message message)
		{
		    var bm = _messages.Insert(() =>
		    {
 				var messageStatus = MessageStatus.InTransit;
				var persistentMessage = message as PersistentMessage;
				if (persistentMessage != null)
					messageStatus = persistentMessage.Status;

		        _messages.ForColumnType<DateTimeColumn>().Set("timestamp", message.SentAt);
                _messages.ForColumnType<BytesColumn>().Set("data", message.Data);
                _messages.ForColumnType<GuidColumn>().Set("instance_id", message.Id.SourceInstanceId);
                _messages.ForColumnType<GuidColumn>().Set("msg_id", message.Id.MessageIdentifier);
                _messages.ForColumnType<StringColumn>().Set("subqueue", message.SubQueue);
                _messages.ForColumnType<StringColumn>().Set("headers", message.Headers.ToQueryString());
                _messages.ForColumnType<IntColumn>().Set("status", (int)messageStatus);
		    });
		    bm.QueueName = _queueName;

			if (string.IsNullOrEmpty(message.SubQueue) == false &&
				Subqueues.Contains(message.SubQueue) == false)
			{
				_actions.AddSubqueueTo(_queueName, message.SubQueue);
				_subqueues = _subqueues.Union(new[] { message.SubQueue }).ToArray();
			}

			_logger.Debug("Enqueuing msg to '{0}' with subqueue: '{1}'. Id: {2}", _queueName, message.SubQueue, message.Id);
			_changeNumberOfMessages(1);
			return bm;
		}

        public void ClearQueue()
        {
            _actions.ClearTable(_messages);
            _actions.ClearTable(_messageHistory);
        }

		public PersistentMessage Dequeue(string subqueue)
		{
		    var enumerator = _messages.GetEnumerator(new StringValueIndex("by_sub_queue", subqueue));

            while(enumerator.MoveNext())
            {
                var id = _messages.GetMessageId();
			    var status = (MessageStatus) _messages.ForColumnType<IntColumn>().Get("status");

				_logger.Debug("Scanning incoming message {2} on '{0}/{1}' with status {3}",
					_queueName, subqueue, id, status);

				if (status != MessageStatus.ReadyToDeliver)
					continue;

				try
				{
                    _messages.Update(() => _messages.ForColumnType<IntColumn>().Set("status", (int)MessageStatus.Processing));
				}
				catch (EsentErrorException e)
				{
					_logger.Debug("Write conflict on '{0}/{1}' for {2}, skipping message",
									   _queueName, subqueue, id);
					if (e.Error == JET_err.WriteConflict)
						continue;
					throw;
				}
			    var bookmark = enumerator.Current;
			    bookmark.QueueName = _queueName;
				_changeNumberOfMessages(-1);

				_logger.Debug("Dequeuing message {2} from '{0}/{1}'",
								   _queueName, subqueue, id);

                return _messages.ReadMessageWithId<PersistentMessage>(bookmark, _queueName, x =>
                {
                    x.SubQueue = subqueue;
                });
            }

			return null;
		}

		public void SetMessageStatus(MessageBookmark bookmark, MessageStatus status, string subqueue)
		{
            _messages.MoveTo(bookmark);
		    var id = _messages.GetMessageId();
            _messages.Update(() =>
            {
                _messages.ForColumnType<IntColumn>().Set("status", (int)status);
                _messages.ForColumnType<StringColumn>().Set("subqueue", subqueue);
            });
			_logger.Debug("Changing message {0} status to {1} on queue '{2}' and set subqueue to '{3}'",
							   id, status, _queueName, subqueue);
		}

		public void SetMessageStatus(MessageBookmark bookmark, MessageStatus status)
		{
            _messages.MoveTo(bookmark);
		    var id = _messages.GetMessageId();

            _messages.Update(() => _messages.ForColumnType<IntColumn>().Set("status", (int)status));

			_logger.Debug("Changing message {0} status to {1} on {2}",
							   id, status, _queueName);
		}

		public void Dispose()
		{
			if (_messages != null)
				_messages.Dispose();
			if (_messageHistory != null)
				_messageHistory.Dispose();
		}

		public void MoveToHistory(MessageBookmark bookmark)
		{
            _messages.MoveTo(bookmark);
		    var id = _messages.GetMessageId();

		    _messageHistory.Insert(() =>
		    {
		        _messages.ColumnNames.Each(x =>
		        {
		            var columnBytes = _messages.ForColumnType<BytesColumn>().Get(x);
		            _messageHistory.ForColumnType<BytesColumn>().Set(x, columnBytes);
		        });
                _messageHistory.ForColumnType<DateTimeColumn>().Set("moved_to_history_at", DateTime.Now);
		    });
            _messages.Delete();
			_logger.Debug("Moving message {0} on queue {1} to history",
							   id, _queueName);
		}

		public IEnumerable<PersistentMessage> GetAllMessages(string subQueue)
		{
		    var enumerator = _messages.GetEnumerator(new StringValueIndex("by_sub_queue", subQueue));
			while(enumerator.MoveNext())
			{
			    var bookmark = enumerator.Current;
			    bookmark.QueueName = _queueName;
			    yield return _messages.ReadMessageWithId<PersistentMessage>(bookmark, _queueName, x =>
			    {
			        x.SubQueue = subQueue;
			    });
			}
		}

        public IEnumerable<HistoryMessage> GetAllProcessedMessages(int? batchSize = null)
		{
            int count = 0;
            var enumerator = _messageHistory.GetEnumerator();

            while (enumerator.MoveNext() && count++ != batchSize)
            {
                var bookmark = enumerator.Current;
                bookmark.QueueName = _queueName;
                yield return _messageHistory.ReadMessageWithId<HistoryMessage>(bookmark, _queueName, x =>
                {
                    x.MovedToHistoryAt = _messageHistory.ForColumnType<DateTimeColumn>().Get("moved_to_history_at");
                });
			}
		}

        public HistoryMessage GetProcessedMessageById(Guid id)
        {
            var enumerator = _messageHistory.GetEnumerator(new GuidIndex(id, "msg_id"));
            while (enumerator.MoveNext())
            {
                var bookmark = enumerator.Current;
                bookmark.QueueName = _queueName;
                return _messageHistory.ReadMessageWithId<HistoryMessage>(bookmark, _queueName, x =>
                {
                    x.MovedToHistoryAt = _messageHistory.ForColumnType<DateTimeColumn>().Get("moved_to_history_at");
                });
            }
            return null;
        }

        public MessageBookmark MoveTo(string subQueue, PersistentMessage message)
		{
            _messages.MoveTo(message.Bookmark);
		    var id = _messages.GetMessageId();
            var bookmark = _messages.Update(() =>
            {
                _messages.ForColumnType<IntColumn>().Set("status", (int)MessageStatus.SubqueueChanged);
                _messages.ForColumnType<StringColumn>().Set("subqueue", subQueue);
            });
		    bookmark.QueueName = _queueName;
		    _logger.Debug("Moving message {0} to subqueue {1}", id, _queueName);
		    return bookmark;
		}

		public MessageStatus GetMessageStatus(MessageBookmark bookmark)
		{
            _messages.MoveTo(bookmark);
		    return (MessageStatus)_messages.ForColumnType<IntColumn>().Get("status");
		}

		public void Discard(MessageBookmark bookmark)
		{
            Delete(bookmark);
		}

		public PersistentMessage Peek(string subqueue)
		{
		    var enumerator = _messages.GetEnumerator(new StringValueIndex("by_sub_queue", subqueue));

			while(enumerator.MoveNext())
			{
			    var id = _messages.GetMessageId();

			    var status = (MessageStatus) _messages.ForColumnType<IntColumn>().Get("status");

				_logger.Debug("Scanning incoming message {2} on '{0}/{1}' with status {3}",
								   _queueName, subqueue, id, status);

				if (status != MessageStatus.ReadyToDeliver)
					continue;

			    var bookmark = enumerator.Current;
			    bookmark.QueueName = _queueName;

				_logger.Debug("Peeking message {2} from '{0}/{1}'",
								   _queueName, subqueue, id);

			    return _messages.ReadMessageWithId<PersistentMessage>(bookmark, _queueName, x =>
			    {
                    x.Status = MessageStatus.ReadyToDeliver;
			        x.SubQueue = subqueue;
			    });
			}
			return null;
		}

		public PersistentMessage PeekById(MessageId id)
		{
		    var enumerator = _messages.GetEnumerator(new MessageIdIndex(id));

			while(enumerator.MoveNext())
			{
			    var bookmark = enumerator.Current;
			    bookmark.QueueName = _queueName;

			    var status = (MessageStatus) _messages.ForColumnType<IntColumn>().Get("status");

				if (status != MessageStatus.ReadyToDeliver)
					continue;
			    return _messages.ReadMessageWithId<PersistentMessage>(bookmark, _queueName);
			}

			return null;
		}

		public void Delete(MessageBookmark bookmark)
		{
            _messages.MoveTo(bookmark);
            _messages.Delete();
		}

		public void DeleteHistoric(MessageBookmark bookmark)
		{
            _messageHistory.MoveTo(bookmark);
            _messageHistory.Delete();
		}

        public MessageBookmark GetMessageHistoryBookmarkAtPosition(int positionFromNewestProcessedMessage)
        {
            var enumerator = _messageHistory.GetEnumerator(new PositionalIndexFromLast(positionFromNewestProcessedMessage));
            return enumerator.MoveNext() ? enumerator.Current : null;
        }
	}
}
