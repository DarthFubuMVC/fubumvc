using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<LightningQueuesServiceRegistry>();
        }
    }

    public class LightningQueuesServiceRegistry : ServiceRegistry
    {
        public LightningQueuesServiceRegistry()
        {
            AddService<ITransport, LightningQueuesTransport>();
            //AddService<IQueueMessageRetrieval, QueueMessageRetrieval>(); // For diagnostics
            SetServiceIfNone<IPersistentQueues, PersistentQueues>().Singleton();
        }
    }
}