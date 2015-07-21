using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    public class InMemoryChannel : IChannel
    {
        public static readonly string Protocol = "memory";
        private readonly InMemoryQueue _queue;

        public InMemoryChannel(Uri address)
        {
            Address = address;
            _queue = InMemoryQueueManager.QueueFor(Address);
        }

        public void Dispose()
        {
            _queue.Dispose();
        }

        public Uri Address { get; private set; }
        public ReceivingState Receive(IReceiver receiver)
        {
            _queue.Receive(receiver);
            return ReceivingState.StopReceiving;
        }

        public void Send(byte[] data, IHeaders headers)
        {
            var envelope = new EnvelopeToken
            {
                Data = data,
                Headers = headers
            };

            _queue.Enqueue(envelope);
        }
    }
}