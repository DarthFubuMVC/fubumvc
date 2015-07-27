using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesReplyChannel : IChannel
    {
        private readonly IQueueManager _queueManager;

        public LightningQueuesReplyChannel(Uri destination, IQueueManager queueManager)
        {
            _queueManager = queueManager;
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
            _queueManager.Send(data, headers, Address);
        }
    }
}