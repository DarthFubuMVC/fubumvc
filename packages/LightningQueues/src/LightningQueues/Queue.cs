using System;
using System.Net;
using LightningQueues.Model;

namespace LightningQueues
{
    public class Queue : IQueue
    {
        private readonly IQueueManager queueManager;
        private readonly string queueName;

        public Queue(IQueueManager queueManager, string queueName)
        {
            this.queueManager = queueManager;
            this.queueName = queueName;
        }

        public IPEndPoint Endpoint
        {
            get { return queueManager.Endpoint; }
        }

        public string QueueName
        {
            get { return queueName; }
        }

        public PersistentMessage[] GetAllMessages(string subqueue)
        {
            return queueManager.GetAllMessages(queueName, subqueue);
        }

        public HistoryMessage[] GetAllProcessedMessages()
        {
            return queueManager.GetAllProcessedMessages(queueName);
        }

        public Message Peek()
        {
            return queueManager.Peek(queueName);
        }

        public Message Peek(TimeSpan timeout)
        {
            return queueManager.Peek(queueName, timeout);
        }

        public Message Peek(string subqueue)
        {
            return queueManager.Peek(queueName, subqueue);
        }

        public Message Peek(string subqueue, TimeSpan timeout)
        {
            return queueManager.Peek(queueName, subqueue, timeout);
        }

        public Message Receive()
        {
            return queueManager.Receive(queueName);
        }

        public Message Receive(TimeSpan timeout)
        {
            return Receive(queueName, timeout);
        }

        public Message Receive(string subqueue)
        {
            return queueManager.Receive(queueName, subqueue);
        }

        public Message Receive(string subqueue, TimeSpan timeout)
        {
            return queueManager.Receive(queueName, subqueue, timeout);
        }

        public void MoveTo(string subqueue, Message message)
        {
            queueManager.MoveTo(subqueue, message);
        }

        public void EnqueueDirectlyTo(string subqueue, MessagePayload payload)
        {
            queueManager.EnqueueDirectlyTo(queueName, subqueue, payload);
        }

        public Message PeekById(MessageId id)
        {
            return queueManager.PeekById(queueName, id);
        }

    	public string[] GetSubqeueues()
    	{
    		return queueManager.GetSubqueues(queueName);
    	}
    }
}
