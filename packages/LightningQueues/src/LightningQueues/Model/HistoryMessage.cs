using System;

namespace LightningQueues.Model
{
    public class HistoryMessage : PersistentMessage
    {
        public DateTime MovedToHistoryAt { get; set; }
    }
}