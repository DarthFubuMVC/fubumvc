namespace LightningQueues.Storage
{
    public enum MessageStatus
    {
        None = 0,
        InTransit = 1,
        Processing = 2,
        ReadyToDeliver = 3,
        SubqueueChanged = 4,
        EnqueueWait = 5,
    }
}