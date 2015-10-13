using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class LightningQueuesFubuDiagnostics
    {
        private readonly IPersistentQueues _queues;
        private readonly IQueueMessageRetrieval _queueMessageRetrieval;

        public LightningQueuesFubuDiagnostics(IPersistentQueues queues, IQueueMessageRetrieval queueMessageRetrieval)
        {
            _queues = queues;
            _queueMessageRetrieval = queueMessageRetrieval;
        }

        public QueueManagersVisualization get_queues()
        {
            var visualization = new QueueManagersVisualization
            {
                QueueManagers = _queues.AllQueueManagers.Select(x => new QueueManagerModel(x)).ToList()
            };

            return visualization;
        }

        public QueueMessagesVisualization get_messages_Port_QueueName(MessagesInputModel input)
        {
            var request = new QueueMessageRetrievalRequest
            {
                Port = input.Port,
                QueueName = input.QueueName
            };

            var messages = _queueMessageRetrieval.GetAllMessagesInQueue(request).Select(msg => new MessageSummary
            {
                id = msg.Id.ToString(),
                status = msg.Status.ToString(),
                sentat = msg.SentAt.ToString(),
                sourceinstanceid = msg.Id.SourceInstanceId.ToString(),
                headers = msg.Headers.ToDictionary()
            }).ToArray();

            return new QueueMessagesVisualization
            {
                Port = input.Port,
                QueueName = input.QueueName,
                Messages = messages
            };
        }
    }

    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
            collection.AllKeys.Each(key =>
            {
                dict.Add(key, collection[key]);
            });

            return dict;
        }
    }

    public class MessageSummary
    {
        public string id { get; set; }
        public string status { get; set; }
        public string sentat { get; set; }
        public string sourceinstanceid { get; set; }
        public IDictionary<string, string> headers { get; set; }
    }

    public class QueueMessagesVisualization
    {
        public string QueueName { get; set; }
        public MessageSummary[] Messages { get; set; }
        public int Port { get; set; }
    }

    public class MessagesInputModel
    {
        public int Port { get; set; }
        public string QueueName { get; set; }
    }

    public class QueueDto
    {
        public string QueueName { get; set; }
        public int Port { get; set; }
        public int NumberOfMessages { get; set; }
    }

    public class QueueManagersVisualization
    {
        public IList<QueueManagerModel> QueueManagers { get; set; }
    }
}