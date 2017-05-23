using System;

namespace LightningQueues.Storage
{
    public interface ITransaction
    {
        Guid TransactionId { get; }
        void Commit();
        void Rollback();
    }
}
