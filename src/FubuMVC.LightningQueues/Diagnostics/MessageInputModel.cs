using System;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class MessageInputModel
    {
        public string MessageId { get; set; }
        public string SourceInstanceId { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
    }
}
