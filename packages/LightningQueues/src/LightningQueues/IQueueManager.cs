using System;
using System.Collections.Generic;
using System.Net;
using LightningQueues.Internal;
using LightningQueues.Model;

namespace LightningQueues
{
    public interface IQueueManager : IDisposable
    {
        QueueManagerConfiguration Configuration { get; set; }
        string Path { get; }
        IPEndPoint Endpoint { get; }
        string[] Queues { get; }
        ITransactionalScope BeginTransactionalScope();
        void WaitForAllMessagesToBeSent();
        IQueue GetQueue(string queue);
        PersistentMessage[] GetAllMessages(string queueName, string subqueue);
        HistoryMessage[] GetAllProcessedMessages(string queueName);
        HistoryMessage GetProcessedMessageById(string queueName, MessageId id);
        PersistentMessageToSend[] GetAllSentMessages();
        PersistentMessageToSend GetSentMessageById(MessageId id);
        PersistentMessageToSend[] GetMessagesCurrentlySending();
        PersistentMessageToSend GetMessageCurrentlySendingById(MessageId id);
        Message Peek(string queueName, string subqueue, TimeSpan timeout);
        Message Receive(string queueName, string subqueue, TimeSpan timeout);
        IEnumerable<StreamedMessage> ReceiveStream(string queueName);
        IEnumerable<StreamedMessage> ReceiveStream(string queueName, string subqueue);
        Message Receive(ITransaction transaction, string queueName, string subqueue, TimeSpan timeout);
        Message ReceiveById(string queueName, MessageId id);
        Message ReceiveById(ITransaction transaction, string queueName, MessageId id);
        MessageId Send(Uri uri, MessagePayload payload);
        MessageId Send(ITransaction transaction, Uri uri, MessagePayload payload);
        void CreateQueues(params string[] queueNames);
        void MoveTo(string subqueue, Message message);
        void EnqueueDirectlyTo(string queue, string subqueue, MessagePayload payload, MessageId id = null);
        void EnqueueDirectlyTo(ITransaction transaction, string queue, string subqueue, MessagePayload payload, MessageId id = null);
        PersistentMessage PeekById(string queueName, MessageId id);
        string[] GetSubqueues(string queueName);
        int GetNumberOfMessages(string queueName);
        void PurgeOldData();
    }
}
