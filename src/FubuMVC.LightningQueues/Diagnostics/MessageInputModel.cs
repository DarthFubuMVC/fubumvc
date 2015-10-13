using System;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class MessageInputModel
    {
        public Guid MessageId { get; set; }
        public Guid SourceInstanceId { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
    }
}