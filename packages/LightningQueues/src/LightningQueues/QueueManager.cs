using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Transactions;
using LightningQueues.Exceptions;
using LightningQueues.Internal;
using LightningQueues.Logging;
using LightningQueues.Model;
using LightningQueues.Protocol;
using LightningQueues.Storage;
using LightningQueues.Utils;

#pragma warning disable 420
namespace LightningQueues
{
    public class QueueManager : IQueueManager
    {
        [ThreadStatic]
        private static TransactionEnlistment _enlistment;

        [ThreadStatic]
        private static Transaction _currentlyEnslistedTransaction;

        private volatile bool _wasStarted;
        private volatile bool _wasDisposed;
        private volatile int _currentlyInCriticalReceiveStatus;
        private volatile int _currentlyInsideTransaction;
        private readonly IPEndPoint _endpoint;
        private readonly object _newMessageArrivedLock = new object();
        private readonly string _path;
        private readonly ILogger _logger;
        private readonly QueueStorage _queueStorage;

        private Receiver _receiver;
        private Thread _sendingThread;
        private QueuedMessagesSender _queuedMessagesSender;
        private SendingChoke _choke;
        private volatile bool _waitingForAllMessagesToBeSent;


        private readonly ThreadSafeSet<MessageId> _receivedMsgs = new ThreadSafeSet<MessageId>();
        private bool _disposing;

        public QueueManagerConfiguration Configuration { get; set; }

        public ISendingThrottle SendingThrottle { get { return _choke; } }

        public QueueManager(IPEndPoint endpoint, string path, QueueManagerConfiguration configuration, ILogger logger = null)
        {
            Configuration = configuration;

            _endpoint = endpoint;
            _path = path;
            _logger = logger ?? LogManager.GetLogger(GetType());
            _queueStorage = new QueueStorage(path, configuration);
            _queueStorage.Initialize();

            _queueStorage.Global(actions =>
            {
                _receivedMsgs.Add(actions.GetAlreadyReceivedMessageIds());
            });

            HandleRecovery();
        }

        public void Start()
        {
            AssertNotDisposedOrDisposing();

            if (_wasStarted)
                throw new InvalidOperationException("The Start method may not be invoked more than once.");

            _receiver = new Receiver(_endpoint, AcceptMessages);
            _receiver.Start();

            _choke = new SendingChoke();
            _queuedMessagesSender = new QueuedMessagesSender(_queueStorage, _choke, _logger);
            _sendingThread = new Thread(_queuedMessagesSender.Send)
            {
                IsBackground = true,
                Name = "Lightning Queues Sender Thread for " + _path
            };
            _sendingThread.Start();
            _wasStarted = true;
        }

        public void PurgeOldData()
        {
            var receivedIdsPurged = _queueStorage.PurgeHistory();
            _receivedMsgs.Remove(receivedIdsPurged);
        }

        private void HandleRecovery()
        {
            var recoveryRequired = false;
            _queueStorage.Global(actions =>
            {
                actions.MarkAllOutgoingInFlightMessagesAsReadyToSend();
                actions.MarkAllProcessedMessagesWithTransactionsNotRegisterForRecoveryAsReadyToDeliver();
                foreach (var bytes in actions.GetRecoveryInformation())
                {
                    recoveryRequired = true;
                    TransactionManager.Reenlist(_queueStorage.Id, bytes,
                        new TransactionEnlistment(_queueStorage, () => { }, () => { }));
                }
            });
            if (recoveryRequired)
                TransactionManager.RecoveryComplete(_queueStorage.Id);
        }

        public ITransactionalScope BeginTransactionalScope()
        {
            return new TransactionalScope(this, new QueueTransaction(_queueStorage, OnTransactionComplete, AssertNotDisposed));
        }

        public string Path
        {
            get { return _path; }
        }

        public IPEndPoint Endpoint
        {
            get { return _endpoint; }
        }

        public void Dispose()
        {
            if (_wasDisposed)
                return;

            DisposeResourcesWhoseDisposalCannotFail();

            _queueStorage.Dispose();

            // only after we finish incoming recieves, and finish processing
            // active transactions can we mark it as disposed
            _wasDisposed = true;
        }

        private void DisposeResourcesWhoseDisposalCannotFail()
        {
            _disposing = true;

            lock (_newMessageArrivedLock)
            {
                Monitor.PulseAll(_newMessageArrivedLock);
            }

            if (_wasStarted)
            {
                _queuedMessagesSender.Stop();
                _sendingThread.Join();

                _receiver.Dispose();
            }

            while (_currentlyInCriticalReceiveStatus > 0)
            {
                _logger.Info("Waiting for {0} messages that are currently in critical receive status", _currentlyInCriticalReceiveStatus);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            while (_currentlyInsideTransaction > 0)
            {
                _logger.Info("Waiting for {0} transactions currently running", _currentlyInsideTransaction);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private void OnTransactionComplete()
        {
            lock (_newMessageArrivedLock)
            {
                Monitor.PulseAll(_newMessageArrivedLock);
            }
            Interlocked.Decrement(ref _currentlyInsideTransaction);
        }

        private void AssertNotDisposed()
        {
            if (_wasDisposed)
                throw new ObjectDisposedException("QueueManager");
        }


        private void AssertNotDisposedOrDisposing()
        {
            if (_disposing || _wasDisposed)
                throw new ObjectDisposedException("QueueManager");
        }

        public void WaitForAllMessagesToBeSent()
        {
            _waitingForAllMessagesToBeSent = true;
            try
            {
                bool hasMessagesToSend;
                do
                {
                    hasMessagesToSend = _queueStorage.Send(actions => actions.HasMessagesToSend());
                    if (hasMessagesToSend)
                        Thread.Sleep(100);
                } while (hasMessagesToSend);
            }
            finally
            {
                _waitingForAllMessagesToBeSent = false;
            }
        }

        public IQueue GetQueue(string queue)
        {
            return new Queue(this, queue);
        }

        public PersistentMessage[] GetAllMessages(string queueName, string subqueue)
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Global(actions => actions.GetQueue(queueName).GetAllMessages(subqueue).ToArray());
        }

        public HistoryMessage[] GetAllProcessedMessages(string queueName)
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Global(actions => actions.GetQueue(queueName).GetAllProcessedMessages().ToArray());
        }

        public HistoryMessage GetProcessedMessageById(string queueName, MessageId id)
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Global(actions => actions.GetQueue(queueName).GetProcessedMessageById(id.MessageIdentifier));
        }

        public PersistentMessageToSend[] GetAllSentMessages()
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Global(actions => actions.GetSentMessages().ToArray());
        }

        public PersistentMessageToSend GetSentMessageById(MessageId id)
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Global(actions => actions.GetSentMessageById(id.MessageIdentifier));
        }

        public PersistentMessageToSend[] GetMessagesCurrentlySending()
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Send(actions => actions.GetMessagesToSend().ToArray());
        }

        public PersistentMessageToSend GetMessageCurrentlySendingById(MessageId id)
        {
            AssertNotDisposedOrDisposing();
            return _queueStorage.Send(actions => actions.GetMessageToSendById(id.MessageIdentifier));
        }

        public Message Peek(string queueName, string subqueue, TimeSpan timeout)
        {
            var remaining = timeout;
            while (true)
            {
                var message = PeekMessageFromQueue(queueName, subqueue);
                if (message != null)
                    return message;

                lock (_newMessageArrivedLock)
                {
                    message = PeekMessageFromQueue(queueName, subqueue);
                    if (message != null)
                        return message;

                    var sp = Stopwatch.StartNew();
                    if (Monitor.Wait(_newMessageArrivedLock, remaining) == false)
                        throw new TimeoutException("No message arrived in the specified timeframe " + timeout);
                    remaining = (new[] { TimeSpan.Zero, remaining - sp.Elapsed }).Max();
                }
            }
        }

        public Message Receive(string queueName, string subqueue, TimeSpan timeout)
        {
            EnsureEnlistment();

            return Receive(_enlistment, queueName, subqueue, timeout);
        }

        public Message ReceiveById(string queueName, MessageId id)
        {
            EnsureEnlistment();

            return ReceiveById(_enlistment, queueName, id);
        }

        public Message ReceiveById(ITransaction transaction, string queueName, MessageId id)
        {
            var message = _queueStorage.Global(actions =>
            {
                var queue = actions.GetQueue(queueName);

                var msg = queue.PeekById(id);
                if (msg == null)
                    return null;
                queue.SetMessageStatus(msg.Bookmark, MessageStatus.Processing);
                actions.RegisterUpdateToReverse(transaction.Id, msg.Bookmark, MessageStatus.ReadyToDeliver, null);

                return msg;
            });
            return message;
        }

        public MessageId Send(Uri uri, MessagePayload payload)
        {
            if (_waitingForAllMessagesToBeSent)
                throw new CannotSendWhileWaitingForAllMessagesToBeSentException("Currently waiting for all messages to be sent, so we cannot send. You probably have a race condition in your application.");

            EnsureEnlistment();

            return Send(_enlistment, uri, payload);
        }

        private void EnsureEnlistment()
        {
            AssertNotDisposedOrDisposing();

            if (Transaction.Current == null)
                throw new InvalidOperationException("You must use TransactionScope when using LightningQueues");

            if (_currentlyEnslistedTransaction == Transaction.Current)
                return;
            // need to change the enlistment
            Interlocked.Increment(ref _currentlyInsideTransaction);
            _enlistment = new TransactionEnlistment(_queueStorage, OnTransactionComplete, AssertNotDisposed);
            _currentlyEnslistedTransaction = Transaction.Current;
        }

        private PersistentMessage GetMessageFromQueue(ITransaction transaction, string queueName, string subqueue)
        {
            AssertNotDisposedOrDisposing();
            var message = _queueStorage.Global(actions =>
            {
                var msg = actions.GetQueue(queueName).Dequeue(subqueue);

                if (msg != null)
                {
                    actions.RegisterUpdateToReverse(
                        transaction.Id,
                        msg.Bookmark,
                        MessageStatus.ReadyToDeliver,
                        subqueue);
                }
                return msg;
            });

            if (message != null)
                _logger.MessageReceived(message);
            return message;
        }

        private PersistentMessage PeekMessageFromQueue(string queueName, string subqueue)
        {
            AssertNotDisposedOrDisposing();
            var message = _queueStorage.Global(actions => actions.GetQueue(queueName).Peek(subqueue));
            if (message != null)
            {
                _logger.Debug("Peeked message with id '{0}' from '{1}/{2}'",
                                   message.Id, queueName, subqueue);
            }
            return message;
        }

        protected virtual IMessageAcceptance AcceptMessages(Message[] msgs)
        {
            var bookmarks = _queueStorage.Global(actions => _receivedMsgs.Filter(msgs, message => message.Id)
                .Select(x => acceptedBookmarks(actions, x))
                .ToList());
            return new MessageAcceptance(this, bookmarks, msgs, _queueStorage);
        }

        private MessageBookmark acceptedBookmarks(GlobalActions actions, Message message)
        {
            var queue = actions.GetQueue(message.Queue);
            var bookmark = queue.Enqueue(message);
            return bookmark;
        }

        #region Nested type: MessageAcceptance

        private class MessageAcceptance : IMessageAcceptance
        {
            private readonly IList<MessageBookmark> _bookmarks;
            private readonly IEnumerable<Message> _messages;
            private readonly QueueManager _parent;
            private readonly QueueStorage _queueStorage;

            public MessageAcceptance(QueueManager parent,
                IList<MessageBookmark> bookmarks,
                IEnumerable<Message> messages,
                QueueStorage queueStorage)
            {
                _parent = parent;
                _bookmarks = bookmarks;
                _messages = messages;
                _queueStorage = queueStorage;
                Interlocked.Increment(ref parent._currentlyInCriticalReceiveStatus);
            }

            #region IMessageAcceptance Members

            public void Commit()
            {
                try
                {
                    _parent.AssertNotDisposed();
                    _queueStorage.Global(actions =>
                    {
                        _bookmarks.Select(bookmark => new { Queue = actions.GetQueue(bookmark.QueueName), bookmark })
                            .Each(x => x.Queue.SetMessageStatus(x.bookmark, MessageStatus.ReadyToDeliver));

                        _messages.Each(x => actions.MarkReceived(x.Id));
                    });
                    _parent._receivedMsgs.Add(_messages.Select(m => m.Id));

                    _messages.Each(x => _parent._logger.QueuedForReceive(x));

                    lock (_parent._newMessageArrivedLock)
                    {
                        Monitor.PulseAll(_parent._newMessageArrivedLock);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _parent._currentlyInCriticalReceiveStatus);
                }
            }

            public void Abort()
            {
                try
                {
                    _parent.AssertNotDisposed();
                    _queueStorage.Global(actions =>
                    {
                        foreach (var bookmark in _bookmarks)
                        {
                            actions.GetQueue(bookmark.QueueName)
                                .Discard(bookmark);
                        }
                    });
                }
                finally
                {
                    Interlocked.Decrement(ref _parent._currentlyInCriticalReceiveStatus);
                }
            }

            #endregion
        }

        #endregion

        public void CreateQueues(params string[] queueNames)
        {
            AssertNotDisposedOrDisposing();

            _queueStorage.Global(actions =>
            {
                foreach (var queueName in queueNames)
                {
                    actions.CreateQueueIfDoesNotExists(queueName);
                }

            });
        }

        public string[] Queues
        {
            get
            {
                AssertNotDisposedOrDisposing();
                string[] queues = null;
                _queueStorage.Global(actions =>
                {
                    queues = actions.GetAllQueuesNames();

                });
                return queues;
            }
        }

        public void MoveTo(string subqueue, Message message)
        {
            AssertNotDisposedOrDisposing();
            EnsureEnlistment();

            _queueStorage.Global(actions =>
            {
                var queue = actions.GetQueue(message.Queue);
                var bookmark = queue.MoveTo(subqueue, (PersistentMessage)message);
                actions.RegisterUpdateToReverse(_enlistment.Id,
                    bookmark, MessageStatus.ReadyToDeliver,
                    message.SubQueue
                    );
                message.SubQueue = subqueue;
            });

            if (((PersistentMessage)message).Status == MessageStatus.ReadyToDeliver)
                _logger.MessageReceived(message);

            _logger.QueuedForReceive(message);
        }

        public void ClearAllMessages()
        {
            _queueStorage.ClearAllMessages();
        }

        public void EnqueueDirectlyTo(string queue, string subqueue, MessagePayload payload, MessageId id = null)
        {
            EnsureEnlistment();

            EnqueueDirectlyTo(_enlistment, queue, subqueue, payload);
        }

        public void EnqueueDirectlyTo(ITransaction transaction, string queue, string subqueue, MessagePayload payload, MessageId id = null)
        {
            var message = new PersistentMessage
            {
                Data = payload.Data,
                Headers = payload.Headers,
                Id = id ?? new MessageId
                {
                    SourceInstanceId = _queueStorage.Id,
                    MessageIdentifier = GuidCombGenerator.Generate()
                },
                Queue = queue,
                SentAt = DateTime.Now,
                SubQueue = subqueue,
                Status = MessageStatus.EnqueueWait
            };

            _queueStorage.Global(actions =>
            {
                var queueActions = actions.GetQueue(queue);

                var bookmark = queueActions.Enqueue(message);
                actions.RegisterUpdateToReverse(transaction.Id, bookmark, MessageStatus.EnqueueWait, subqueue);

            });

            _logger.QueuedForReceive(message);

            lock (_newMessageArrivedLock)
            {
                Monitor.PulseAll(_newMessageArrivedLock);
            }
        }

        public PersistentMessage PeekById(string queueName, MessageId id)
        {
            PersistentMessage message = null;
            _queueStorage.Global(actions =>
            {
                var queue = actions.GetQueue(queueName);

                message = queue.PeekById(id);

            });
            return message;
        }

        public string[] GetSubqueues(string queueName)
        {
            string[] result = null;
            _queueStorage.Global(actions =>
            {
                var queue = actions.GetQueue(queueName);

                result = queue.Subqueues;

            });
            return result;
        }

        public int GetNumberOfMessages(string queueName)
        {
            int numberOfMsgs = 0;
            _queueStorage.Global(actions =>
            {
                numberOfMsgs = actions.GetNumberOfMessages(queueName);
            });
            return numberOfMsgs;
        }

        public Message Receive(ITransaction transaction, string queueName, string subqueue, TimeSpan timeout)
        {
            var remaining = timeout;
            while (true)
            {
                var message = GetMessageFromQueue(transaction, queueName, subqueue);
                if (message != null)
                {
                    return message;
                }
                lock (_newMessageArrivedLock)
                {
                    message = GetMessageFromQueue(transaction, queueName, subqueue);
                    if (message != null)
                    {
                        return message;
                    }
                    var sp = Stopwatch.StartNew();
                    if (Monitor.Wait(_newMessageArrivedLock, remaining) == false)
                        throw new TimeoutException("No message arrived in the specified timeframe " + timeout);
                    var newRemaining = remaining - sp.Elapsed;
                    remaining = newRemaining >= TimeSpan.Zero ? newRemaining : TimeSpan.Zero;
                }
            }
        }

        public IEnumerable<StreamedMessage> ReceiveStream(string queueName)
        {
            return ReceiveStream(queueName, null);
        }

        public IEnumerable<StreamedMessage> ReceiveStream(string queueName, string subqueue)
        {
            while (!_disposing)
            {
                var peekedMessage = PeekMessageFromQueue(queueName, subqueue);
                if (peekedMessage == null)
                {
                    lock (_newMessageArrivedLock)
                    {
                        Monitor.Wait(_newMessageArrivedLock, TimeSpan.FromSeconds(1));
                        continue;
                    }
                }

                Interlocked.Increment(ref _currentlyInsideTransaction);
                var scope = BeginTransactionalScope();
                var message = GetMessageFromQueue(scope.Transaction, queueName, subqueue);
                if (message != null)
                {
                    yield return new StreamedMessage { Message = message, TransactionalScope = scope };
                    continue;
                }
                scope.Rollback();
            }
        }

        public MessageId Send(ITransaction transaction, Uri uri, MessagePayload payload)
        {
            if (_waitingForAllMessagesToBeSent)
                throw new CannotSendWhileWaitingForAllMessagesToBeSentException("Currently waiting for all messages to be sent, so we cannot send. You probably have a race condition in your application.");

            var parts = uri.AbsolutePath.Substring(1).Split('/');
            var queue = parts[0];
            string subqueue = null;
            if (parts.Length > 1)
            {
                subqueue = string.Join("/", parts.Skip(1).ToArray());
            }

            Guid msgId = Guid.Empty;

            var port = uri.Port;
            if (port == -1)
                port = 2200;
            var destination = new Endpoint(uri.Host, port);

            _queueStorage.Global(actions =>
            {
                msgId = actions.RegisterToSend(destination, queue,
                                               subqueue, payload, transaction.Id);

            });

            var messageId = new MessageId
            {
                SourceInstanceId = _queueStorage.Id,
                MessageIdentifier = msgId
            };
            var message = new Message
            {
                Id = messageId,
                Data = payload.Data,
                Headers = payload.Headers,
                Queue = queue,
                SubQueue = subqueue
            };

            _logger.QueuedForSend(message, destination);

            return messageId;
        }
    }
}
#pragma warning restore 420
