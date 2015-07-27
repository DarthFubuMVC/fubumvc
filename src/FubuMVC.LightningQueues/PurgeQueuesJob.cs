using System.Collections.Generic;
using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.LightningQueues
{
    public class PurgeQueuesJob : IJob
    {
        private readonly IPersistentQueues _persistentQueues;

        public PurgeQueuesJob(IPersistentQueues persistentQueues)
        {
            _persistentQueues = persistentQueues;
        }

        public void Execute(CancellationToken cancellation)
        {
            _persistentQueues.AllQueueManagers.Each(x => x.PurgeOldData());
        }
    }
}