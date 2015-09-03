using LightningQueues.Protocol;
using LightningQueues.Storage;

namespace LightningQueues.Model
{
    public class PersistentMessageToSend : PersistentMessage
    {
        public OutgoingMessageStatus OutgoingStatus { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}