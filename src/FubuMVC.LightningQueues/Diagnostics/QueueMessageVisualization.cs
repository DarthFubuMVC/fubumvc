using System;
using System.Collections.Generic;
using FubuMVC.LightningQueues.Queues;

namespace FubuMVC.LightningQueues.Diagnostics
{

    public class QueueMessageVisualization
    {
        public MessageId MessageId { get; set; }
        public string QueueName { get; set; }
        public DateTime SentAt { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }

        public bool NotFound { get; set; }
        public int Port { get; set; }
    }
}
