using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Urls;
using HtmlTags;
using LightningQueues;

namespace FubuTransportation.LightningQueues.Diagnostics
{
    public class LightningQueuesFubuDiagnostics
    {
        private readonly IPersistentQueues _queues;
        private readonly IUrlRegistry _urls;

        public LightningQueuesFubuDiagnostics(IPersistentQueues queues, IUrlRegistry urls)
        {
            _queues = queues;
            _urls = urls;
        }

        public QueueManagersVisualization get_queue__managers()
        {
            var visualization = new QueueManagersVisualization
            {
                QueueManagers = _queues.AllQueueManagers.Select(x => new QueueManagerModel(x, _urls)).ToList()
            };

            return visualization;
        }
    }

    public class QueueManagerModel
    {
        public QueueManagerModel(IQueueManager queueManager, IUrlRegistry urls)
        {
            Queues = new QueueManagerTableTag(queueManager, urls);
            EnableProcessedMessageHistory = queueManager.Configuration.EnableProcessedMessageHistory;
            EnableOutgoingMessageHistory = queueManager.Configuration.EnableOutgoingMessageHistory;
            Path = queueManager.Path;
            Port = queueManager.Endpoint.Port;
            OldestMessageInOutgoingHistory = queueManager.Configuration.OldestMessageInOutgoingHistory;
            OldestMessageInProcessedHistory = queueManager.Configuration.OldestMessageInProcessedHistory;
            NumberOfMessagesToKeepInOutgoingHistory = queueManager.Configuration.NumberOfMessagesToKeepInOutgoingHistory;
            NumberOfMessagesToKeepInProcessedHistory = queueManager.Configuration.NumberOfMessagesToKeepInProcessedHistory;
            NumberOfMessagIdsToKeep = queueManager.Configuration.NumberOfReceivedMessageIdsToKeep;
        }

        public int Port { get; set; }
        public string Path { get; set; }
        public bool EnableProcessedMessageHistory { get; set; }
        public bool EnableOutgoingMessageHistory { get; set; }
        public TimeSpan OldestMessageInOutgoingHistory { get; set; }
        public TimeSpan OldestMessageInProcessedHistory { get; set; }
        public int NumberOfMessagesToKeepInOutgoingHistory { get; set; }
        public int NumberOfMessagesToKeepInProcessedHistory { get; set; }
        public int NumberOfMessagIdsToKeep { get; set; }
        public QueueManagerTableTag Queues { get; set; }
    }

    public class QueueManagersVisualization
    {
        public IList<QueueManagerModel> QueueManagers { get; set; }
    }

    public class QueueManagerTableTag : TableTag
    {
        public QueueManagerTableTag(IQueueManager queueManager, IUrlRegistry urls)
        {
            AddClass("table");

            AddHeaderRow(x =>
            {
                x.Header("Queue Name");
                x.Header("Message Count");
            });

            queueManager.Queues.Each(queueName =>
            {
                AddBodyRow(row => addQueueRow(row, queueManager, queueName, urls));
                AddBodyRow(row => addQueueRow(row, queueManager, "{0}_history".ToFormat(queueName), urls, "N/A"));
            });
            AddBodyRow(row => addQueueRow(row, queueManager, "outgoing", urls, "N/A"));
            AddBodyRow(row => addQueueRow(row, queueManager, "outgoing_history", urls, "N/A"));
        }

        private void addQueueRow(TableRowTag row, IQueueManager queueManager, string queueName, IUrlRegistry urls, string displayForCount = null)
        {
            var url = urls.UrlFor(new MessagesInputModel {Port = queueManager.Endpoint.Port, QueueName = queueName});

            row.Cell().Add("a")
                .Attr("href", url)
                .Text(queueName);
            
            row.Cell(displayForCount ?? queueManager.GetNumberOfMessages(queueName).ToString(CultureInfo.InvariantCulture));
        }
    }
}