using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests;
using LightningQueues;
using Xunit;
using StructureMap;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class LightningQueuesServiceRegistry_spec
    {
        [Fact]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<LightningQueueSettings>(_ => _.DisableIfNoChannels = true);

            using (var runtime = FubuRuntime.Basic())
            {
                var container = runtime.Get<IContainer>();

                container.ShouldHaveRegistration<ITransport, LightningQueuesTransport>();
                container.DefaultRegistrationIs<IPersistentQueues, PersistentQueues>();
            }
        }
    }
}