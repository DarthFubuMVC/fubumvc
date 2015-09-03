using System;
using System.Collections.Specialized;

namespace LightningQueues.Model
{
    public class Message
    {
        public Message()
        {
            Headers = new NameValueCollection();
        }

        public MessageId Id { get; set; }
        public string Queue { get; set; }
        public DateTime SentAt { get; set; }
        public NameValueCollection Headers { get; set; }
        public byte[] Data { get; set; }
        public string SubQueue { get; set; }
    }
}