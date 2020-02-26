using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LightningQueues;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class QueueMessageRetrieval : IQueueMessageRetrieval
    {
        private readonly IPersistentQueues _queues;

        private static readonly IEnumerable<QueueMessagesRetrievalStrategy> MessageRetrievalStrategies =
            new List<QueueMessagesRetrievalStrategy>
            {
                new QueueMessagesRetrievalStrategy
                {
                    CanHandle = req => req.QueueName == "outgoing",
                    Execute = (req, queue) => queue.Store.PersistedOutgoingMessages().ToEnumerable().ToArray(),
                    ExecuteForSingleMessage = (req, queue) => queue.Store.GetMessage("outgoing", req.MessageId)
                },
                new QueueMessagesRetrievalStrategy
                {
                    CanHandle = _ => true,
                    Execute = (req, queue) => queue.Store.PersistedMessages(req.QueueName).ToEnumerable().ToArray(),
                    ExecuteForSingleMessage = (req, queue) => queue.Store.GetMessage(req.QueueName, req.MessageId)
                }
            };

        public QueueMessageRetrieval(IPersistentQueues queues)
        {
            _queues = queues;
        }

        public IEnumerable<Message> GetAllMessagesInQueue(QueueMessageRetrievalRequest request)
        {
            return MessageRetrievalStrategies
                .First(x => x.CanHandle(request))
                .Execute(request, GetQueueManager(request));
        }

        public Message GetSingleMessageInQueue(QueueMessageRetrievalRequest request)
        {
            return MessageRetrievalStrategies
                .First(x => x.CanHandle(request))
                .ExecuteForSingleMessage(request, GetQueueManager(request));
        }

        private Queue GetQueueManager(QueueMessageRetrievalRequest request)
        {
            return _queues.AllQueueManagers.Single(x => x.Endpoint.Port == request.Port);
        }

        private class QueueMessagesRetrievalStrategy
        {
            public Func<QueueMessageRetrievalRequest, bool> CanHandle { get; set; }
            public Func<QueueMessageRetrievalRequest, Queue, IEnumerable<Message>> Execute { get; set; }
            public Func<QueueMessageRetrievalRequest, Queue, Message> ExecuteForSingleMessage { get; set; }
        }
    }

    public interface IQueueMessageRetrieval
    {
        IEnumerable<Message> GetAllMessagesInQueue(QueueMessageRetrievalRequest request);
        Message GetSingleMessageInQueue(QueueMessageRetrievalRequest request);
    }

    public class QueueMessageRetrievalRequest
    {
        public int Port { get; set; }
        public string QueueName { get; set; }
        public MessageId MessageId { get; set; }
    }
}
