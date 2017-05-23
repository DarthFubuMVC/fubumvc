using System;

namespace FubuMVC.LightningQueues.Queues
{
    public interface IQueueContext
    {
        void CommitChanges();
        void Send(OutgoingMessage message);
        void ReceiveLater(TimeSpan timeSpan);
        void ReceiveLater(DateTimeOffset time);
        void SuccessfullyReceived();
        void MoveTo(string queueName);
        void Enqueue(Message message);
    }
}