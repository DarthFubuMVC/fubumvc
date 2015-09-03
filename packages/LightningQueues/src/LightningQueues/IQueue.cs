using System;
using System.Net;
using LightningQueues.Model;

namespace LightningQueues
{
    public interface IQueue
    {
        IPEndPoint Endpoint { get; }
        string QueueName { get; }

        PersistentMessage[] GetAllMessages(string subqueue);
        HistoryMessage[] GetAllProcessedMessages();
        
        Message Peek();
        Message Peek(TimeSpan timeout);
        Message Peek(string subqueue);
        Message Peek(string subqueue, TimeSpan timeout);
        
        Message Receive();
        Message Receive(TimeSpan timeout);
        Message Receive(string subqueue);
        Message Receive(string subqueue, TimeSpan timeout);
        
        void MoveTo(string subqueue, Message message);
        void EnqueueDirectlyTo(string subqueue, MessagePayload payload);

        Message PeekById(MessageId id);
    	string[] GetSubqeueues();
    }
}