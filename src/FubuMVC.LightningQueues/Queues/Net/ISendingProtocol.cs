using System;

namespace FubuMVC.LightningQueues.Queues.Net
{
    public interface ISendingProtocol
    {
        IObservable<OutgoingMessage> Send(OutgoingMessageBatch batch);
    }
}