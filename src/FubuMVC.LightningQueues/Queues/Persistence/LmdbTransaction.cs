using System;
using FubuMVC.LightningQueues.Queues.Storage;
using LightningDB;

namespace FubuMVC.LightningQueues.Queues.Persistence
{
    public class LmdbTransaction : ITransaction
    {
        private readonly LightningTransaction _transaction;

        public LmdbTransaction(LightningEnvironment env)
        {
            _transaction = env.BeginTransaction();
            TransactionId = Guid.NewGuid();
        }

        public LightningTransaction Transaction => _transaction;

        public Guid TransactionId { get; }

        void ITransaction.Rollback()
        {
            _transaction.Dispose();
        }

        void ITransaction.Commit()
        {
            using(_transaction)
                _transaction.Commit();
        }
    }
}