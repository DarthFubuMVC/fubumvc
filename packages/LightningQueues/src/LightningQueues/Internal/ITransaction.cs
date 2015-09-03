using System;

namespace LightningQueues.Internal
{
    public interface ITransaction
    {
        Guid Id { get; }
        void Rollback();
        void Commit();
    }
}