using System;

namespace LightningQueues.Net
{
    public interface ISendingProtocol
    {
        IObservable<OutgoingMessage> Send(OutgoingMessageBatch batch);
    }
}