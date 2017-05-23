using System;

namespace FubuMVC.LightningQueues.Queues.Net
{
    public class OutgoingMessageFailure
    {
        public Exception Exception { get; set; }
        public OutgoingMessageBatch Batch { get; set; }
    }
}