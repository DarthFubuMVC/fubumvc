using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests;
using LightningQueues.Model;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class LightningQueuesServiceRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<LightningQueueSettings>(_ => _.DisableIfNoChannels = true);

            using (var runtime = FubuRuntime.Basic())
            {
                var container = runtime.Get<IContainer>();

                container.ShouldHaveRegistration<ITransport, LightningQueuesTransport>();
                container.DefaultRegistrationIs<IPersistentQueues, PersistentQueues>();
                container.DefaultSingletonIs<IDelayedMessageCache<MessageId>, DelayedMessageCache<MessageId>>();
            }
        }
    }
}