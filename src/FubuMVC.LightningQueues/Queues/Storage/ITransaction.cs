using System;

namespace FubuMVC.LightningQueues.Queues.Storage
{
    public interface ITransaction
    {
        Guid TransactionId { get; }
        void Commit();
        void Rollback();
    }
}
