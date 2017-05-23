using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LightningQueues.Logging;
using LightningQueues.Net;
using LightningQueues.Net.Tcp;
using LightningQueues.Storage;

namespace LightningQueues
{
    public class Queue : IDisposable
    {
        private readonly Sender _sender;
        private readonly Receiver _receiver;
        private readonly IMessageStore _messageStore;
        private readonly Subject<Message> _receiveSubject;
        private readonly Subject<OutgoingMessage> _sendSubject;
        private readonly IScheduler _scheduler;
        private readonly ILogger _logger;

        public Queue(Receiver receiver, Sender sender, IMessageStore messageStore, ILogger logger) : this(receiver, sender, messageStore, TaskPoolScheduler.Default, logger)
        {
        }

        public Queue(Receiver receiver, Sender sender, IMessageStore messageStore, IScheduler scheduler, ILogger logger)
        {
            _receiver = receiver;
            _sender = sender;
            _messageStore = messageStore;
            _receiveSubject = new Subject<Message>();
            _sendSubject = new Subject<OutgoingMessage>();
            _scheduler = scheduler;
            _logger = logger;
        }

        public IPEndPoint Endpoint => _receiver.Endpoint;

        public string[] Queues => _messageStore.GetAllQueues();

        public IMessageStore Store => _messageStore;

        internal ISubject<Message> ReceiveLoop => _receiveSubject;
        internal ISubject<OutgoingMessage> SendLoop => _sendSubject;

        public void CreateQueue(string queueName)
        {
            _messageStore.CreateQueue(queueName);
        }

        public void Start()
        {
            _logger.Debug("Starting LightningQueues");
            var errorPolicy = new SendingErrorPolicy(_logger, _messageStore, _sender.FailedToSend());
            _sender.StartSending(_messageStore.PersistedOutgoingMessages()
                .Merge(_sendSubject)
                .Merge(errorPolicy.RetryStream)
                .ObserveOn(TaskPoolScheduler.Default));
        }

        public IObservable<MessageContext> Receive(string queueName)
        {
            _logger.DebugFormat("Starting to receive for queue {0}", queueName);
            return _messageStore.PersistedMessages(queueName)
                .Concat(_receiver.StartReceiving())
                .Merge(_receiveSubject)
                .Where(x => x.Queue == queueName)
                .Select(x => new MessageContext(x, this));
        }

        public void MoveToQueue(string queueName, Message message)
        {
            _logger.DebugFormat("Moving message {0} to {1}", message.Id.MessageIdentifier, queueName);
            var tx = _messageStore.BeginTransaction();
            _messageStore.MoveToQueue(tx, queueName, message);
            tx.Commit();
            message.Queue = queueName;
            _receiveSubject.OnNext(message);
        }

        public void Enqueue(Message message)
        {
            _logger.DebugFormat("Enqueueing message {0} to queue {1}", message.Id.MessageIdentifier, message.Queue);
            _messageStore.StoreIncomingMessages(message);
            _receiveSubject.OnNext(message);
        }

        public void ReceiveLater(Message message, TimeSpan timeSpan)
        {
            _logger.DebugFormat("Delaying message {0} until {1}", message.Id.MessageIdentifier, timeSpan);
            _scheduler.Schedule(message, timeSpan, (sch, msg) =>
            {
                _receiveSubject.OnNext(msg);
                return Disposable.Empty;
            });
        }

        public void Send(params OutgoingMessage[] messages)
        {
            _logger.DebugFormat("Sending {0} messages", messages.Length);
            var tx = _messageStore.BeginTransaction();
            foreach (var message in messages)
            {
                _messageStore.StoreOutgoing(tx, message);
            }
            tx.Commit();
            foreach (var message in messages)
            {
                _sendSubject.OnNext(message);
            }
        }

        internal void SendImmediate(OutgoingMessage message)
        {
            _sendSubject.OnNext(message);
        }

        public void ReceiveLater(Message message, DateTimeOffset time)
        {
            _logger.DebugFormat("Delaying message {0} until {1}", message.Id.MessageIdentifier, time);
            _scheduler.Schedule(message, time, (sch, msg) =>
            {
                _receiveSubject.OnNext(msg);
                return Disposable.Empty;
            });
        }

        public void Dispose()
        {
            _logger.Info("Disposing queue");
            _messageStore.Dispose();
            try
            {
                _sender.Dispose();
                _receiver.Dispose();
                _receiveSubject.Dispose();
                _sendSubject.Dispose();
            }
            catch (Exception e)
            {
                _logger.Error("Failed when shutting down queue", e);
            }
            
        }
    }
}