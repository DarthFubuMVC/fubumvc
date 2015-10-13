using System.Collections.Generic;
using System.Linq;
using LightningQueues;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class LightningQueuesFubuDiagnostics
    {
        private readonly IPersistentQueues _queues;

        public LightningQueuesFubuDiagnostics(IPersistentQueues queues)
        {
            _queues = queues;
        }

        public QueueManagersVisualization get_queues()
        {
            var visualization = new QueueManagersVisualization
            {
                QueueManagers = _queues.AllQueueManagers.Select(x => new QueueManagerModel(x)).ToList()
            };

            return visualization;
        }
    }

    public class QueueManagerModel
    {
        public QueueManagerModel(IQueueManager queueManager)
        {
            EnableProcessedMessageHistory = queueManager.Configuration.EnableProcessedMessageHistory;
            EnableOutgoingMessageHistory = queueManager.Configuration.EnableOutgoingMessageHistory;
            Path = queueManager.Path;
            Port = queueManager.Endpoint.Port;
            OldestMessageInOutgoingHistory = queueManager.Configuration.OldestMessageInOutgoingHistory.TotalMilliseconds;
            OldestMessageInProcessedHistory =
                queueManager.Configuration.OldestMessageInProcessedHistory.TotalMilliseconds;
            NumberOfMessagesToKeepInOutgoingHistory = queueManager.Configuration.NumberOfMessagesToKeepInOutgoingHistory;
            NumberOfMessagesToKeepInProcessedHistory =
                queueManager.Configuration.NumberOfMessagesToKeepInProcessedHistory;
            NumberOfMessagIdsToKeep = queueManager.Configuration.NumberOfReceivedMessageIdsToKeep;
            Queues = buildQueues(queueManager).ToArray();
        }

        public int Port { get; set; }
        public string Path { get; set; }
        public bool EnableProcessedMessageHistory { get; set; }
        public bool EnableOutgoingMessageHistory { get; set; }
        public double OldestMessageInOutgoingHistory { get; set; }
        public double OldestMessageInProcessedHistory { get; set; }
        public int NumberOfMessagesToKeepInOutgoingHistory { get; set; }
        public int NumberOfMessagesToKeepInProcessedHistory { get; set; }
        public int NumberOfMessagIdsToKeep { get; set; }
        public QueueDto[] Queues { get; set; }

        private IEnumerable<QueueDto> buildQueues(IQueueManager queues)
        {
            foreach (var queue in queues.Queues)
            {
                yield return new QueueDto
                {
                    Port = queues.Endpoint.Port,
                    QueueName = queue,
                    NumberOfMessages = queues.GetNumberOfMessages(queue)
                };
            }
        }
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