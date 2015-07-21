using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuTransportation.LightningQueues.Diagnostics;
using LightningQueues.Model;

namespace FubuTransportation.LightningQueues
{
    public class LightningQueuesExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<LightningQueuesServiceRegistry>();

            registry.Import<LightningQueuesJobRegistry>();
        }
    }

    public class LightningQueuesServiceRegistry : ServiceRegistry
    {
        public LightningQueuesServiceRegistry()
        {
            AddService<ITransport, LightningQueuesTransport>();
            AddService<IQueueMessageRetrieval, QueueMessageRetrieval>(); // For diagnostics
            SetServiceIfNone<IPersistentQueues, PersistentQueues>(x => x.IsSingleton = true);
            SetServiceIfNone<IDelayedMessageCache<MessageId>, DelayedMessageCache<MessageId>>();
        }
    }

    public class LightningQueuesJobRegistry : FubuTransportRegistry
    {
        public LightningQueuesJobRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            Polling.RunJob<PurgeQueuesJob>()
                .ScheduledAtInterval<LightningQueueSettings>(x => x.PurgeQueuesPolling);
        }
    }
}
