using LightningQueues.Storage;

namespace LightningQueues.Model
{
    public class PersistentMessage : Message
    {
        public MessageBookmark Bookmark { get; set; }
        public MessageStatus Status { get; set; }
    }
}