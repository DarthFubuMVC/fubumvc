using System;
using System.Collections.Generic;

namespace LightningQueues
{
    public class Message
    {
        public Message()
        {
            Headers = new Dictionary<string, string>();
            SentAt = DateTime.UtcNow;
        }

        public MessageId Id { get; set; }
        public string Queue { get; set; }
        public DateTime SentAt { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public byte[] Data { get; set; }
        [Obsolete("This is purely for backwards compatibility in wire format")]
        public string SubQueue { get; set; }
    }
}
