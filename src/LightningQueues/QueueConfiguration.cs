using System;
using System.Net;
using System.Reactive.Concurrency;
using LightningQueues.Logging;
using LightningQueues.Net;
using LightningQueues.Net.Protocol.V1;
using LightningQueues.Net.Tcp;
using LightningQueues.Storage;

namespace LightningQueues
{
    public class QueueConfiguration
    {
        private IScheduler _scheduler;
        private IMessageStore _store;
        private IPEndPoint _endpoint;
        private IReceivingProtocol _receivingProtocol;
        private ISendingProtocol _sendingProtocol;
        private ILogger _logger;

        public QueueConfiguration()
        {
            _logger = new NulloLogger();
        }

        public QueueConfiguration StoreMessagesWith(IMessageStore store)
        {
            _store = store;
            return this;
        }

        public QueueConfiguration ReceiveMessagesAt(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        public QueueConfiguration CommunicateWithProtocol(IReceivingProtocol receivingProtocol, ISendingProtocol sendingProtocol)
        {
            _receivingProtocol = receivingProtocol;
            _sendingProtocol = sendingProtocol;
            return this;
        }

        public QueueConfiguration AutomaticEndpoint()
        {
            return ReceiveMessagesAt(new IPEndPoint(IPAddress.Loopback, PortFinder.FindPort()));
        }

        public QueueConfiguration ScheduleQueueWith(IScheduler scheduler)
        {
            _scheduler = scheduler;
            return this;
        }

        public QueueConfiguration LogWith(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public Queue BuildQueue()
        {
            if(_store == null)
                throw new ArgumentNullException(nameof(_store), "Storage has not been configured. Are you missing a call to StoreMessagesWith?");

            if(_endpoint == null)
                throw new ArgumentNullException(nameof(_endpoint), "Endpoint has not been configured. Are you missing a call to ReceiveMessageAt?");

            InitializeDefaults();

            var receiver = new Receiver(_endpoint, _receivingProtocol, _logger);
            var sender = new Sender(_logger, _sendingProtocol);
            var queue = new Queue(receiver, sender, _store, _scheduler, _logger);
            return queue;
        }

        private void InitializeDefaults()
        {
            _sendingProtocol = _sendingProtocol ?? new SendingProtocol(_store, _logger);
            _receivingProtocol = _receivingProtocol ?? new ReceivingProtocol(_store, _logger);
            _scheduler = _scheduler ?? TaskPoolScheduler.Default;
        }
    }
}