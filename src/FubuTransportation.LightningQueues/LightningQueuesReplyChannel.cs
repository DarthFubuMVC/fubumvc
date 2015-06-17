using System;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Headers;
using LightningQueues;

namespace FubuTransportation.LightningQueues
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