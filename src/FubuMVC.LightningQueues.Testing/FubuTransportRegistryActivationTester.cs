using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class FubuTransportRegistryActivationTester
    {
        [Test]
        public void should_start_when_transport_disabled()
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<TransportSettings>(x =>
            {
                x.Enabled = true;
                x.InMemoryTransport = InMemoryTransportMode.Enabled;
            });
            registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);
            using (var application = registry.ToRuntime())
            {
            }
        }
    }
}