using System;

namespace LightningQueues.Storage
{
    public interface IMessageStore : IDisposable
    {
        ITransaction BeginTransaction();
        void CreateQueue(string queueName);
        void StoreIncomingMessages(params Message[] messages);
        void StoreIncomingMessages(ITransaction transaction, params Message[] messages);
        void DeleteIncomingMessages(params Message[] messages);
        IObservable<Message> PersistedMessages(string queueName);
        IObservable<OutgoingMessage> PersistedOutgoingMessages();
        void MoveToQueue(ITransaction transaction, string queueName, Message message);
        void SuccessfullyReceived(ITransaction transaction, Message message);
        void StoreOutgoing(ITransaction tx, OutgoingMessage message);
        int FailedToSend(OutgoingMessage message);
        void SuccessfullySent(params OutgoingMessage[] messages);
        Message GetMessage(string queueName, MessageId messageId);
        string[] GetAllQueues();
        void ClearAllStorage();
    }
}
