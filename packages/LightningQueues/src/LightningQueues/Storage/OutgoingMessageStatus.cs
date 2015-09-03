namespace LightningQueues.Storage
{
    public enum OutgoingMessageStatus
    {
        NotReady = 0,
        Ready = 1,
        InFlight = 2,
        Sent = 4,
        Failed = 5
    }
}