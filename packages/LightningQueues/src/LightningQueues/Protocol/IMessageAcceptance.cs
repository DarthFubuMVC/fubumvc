namespace LightningQueues.Protocol
{
    public interface IMessageAcceptance
    {
        void Commit();
        void Abort();
    }
}