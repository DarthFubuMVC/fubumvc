using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Tests;
using LightningQueues.Model;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class LightningQueuesServiceRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<LightningQueueSettings>(_ => _.DisableIfNoChannels = true);

            using (var runtime = FubuApplication.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<ITransport, LightningQueuesTransport>();
                runtime.Container.DefaultRegistrationIs<IPersistentQueues, PersistentQueues>();
                runtime.Container.DefaultSingletonIs<IDelayedMessageCache<MessageId>, DelayedMessageCache<MessageId>>();
            }
        }
    }
}