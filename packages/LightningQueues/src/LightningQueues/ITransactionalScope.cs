using LightningQueues.Internal;

namespace LightningQueues
{
    public interface ITransactionalScope
    {
        ITransaction Transaction { get; }
        IQueueManager QueueManager { get; }
        void Commit();
        void Rollback();
    }
}