using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.LightningQueues.Diagnostics;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<LightningQueuesServiceRegistry>();
            registry.Polling.RunJob<PurgeQueuesJob>()
                .ScheduledAtInterval<LightningQueueSettings>(x => x.PurgeQueuesPolling);
        }
    }

    public class LightningQueuesServiceRegistry : ServiceRegistry
    {
        public LightningQueuesServiceRegistry()
        {
            AddService<ITransport, LightningQueuesTransport>();
            AddService<IQueueMessageRetrieval, QueueMessageRetrieval>(); // For diagnostics
            SetServiceIfNone<IPersistentQueues, PersistentQueues>().Singleton();
            SetServiceIfNone<IDelayedMessageCache<MessageId>, DelayedMessageCache<MessageId>>().Singleton();
        }
    }


}