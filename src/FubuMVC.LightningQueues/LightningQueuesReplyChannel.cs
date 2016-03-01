using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesReplyChannel : IChannel
    {
        private readonly Queue _queueManager;
        private readonly string _queueName;

        public LightningQueuesReplyChannel(Uri destination, Queue queueManager, string queueName)
        {
            _queueManager = queueManager;
            _queueName = queueName;
            Address = destination;
        }

        public void Dispose()
        {
        }

        public Uri Address { get; private set; }
        public ReceivingState Receive(IReceiver receiver)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data, IHeaders headers)
        {
            _queueManager.Send(data, headers, Address, _queueName);
        }
    }
}