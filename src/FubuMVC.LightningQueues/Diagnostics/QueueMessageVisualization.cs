using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.Services;
using HtmlTags;
using LightningQueues.Model;
using LightningQueues.Storage;

namespace FubuMVC.LightningQueues.Diagnostics
{

    public class QueueMessageVisualization
    {
        public MessageId MessageId { get; set; }
        public string QueueName { get; set; }
        public string SubQueueName { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime SentAt { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }

        public bool NotFound { get; set; }
        public int Port { get; set; }
    }
}