namespace LightningQueues.Model
{
    public class StreamedMessage
    {
        public Message Message { get; set; }
        public ITransactionalScope TransactionalScope { get; set; }
    }
}