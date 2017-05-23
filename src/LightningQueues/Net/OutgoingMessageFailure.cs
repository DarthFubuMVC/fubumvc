using System;

namespace LightningQueues.Net
{
    public class OutgoingMessageFailure
    {
        public Exception Exception { get; set; }
        public OutgoingMessageBatch Batch { get; set; }
    }
}