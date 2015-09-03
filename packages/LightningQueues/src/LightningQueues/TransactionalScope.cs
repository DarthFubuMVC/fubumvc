using System;
using LightningQueues.Internal;
using LightningQueues.Model;

namespace LightningQueues
{
    public class TransactionalScope : ITransactionalScope
    {
        public TransactionalScope(IQueueManager queueManager, ITransaction transaction)
        {
            QueueManager = queueManager;
            Transaction = transaction;
        }

        public ITransaction Transaction { get; private set; }
        public IQueueManager QueueManager { get; private set; }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }
    }
}