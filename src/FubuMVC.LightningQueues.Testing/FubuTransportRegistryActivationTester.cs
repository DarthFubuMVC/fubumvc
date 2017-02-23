using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using Xunit;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class FubuTransportRegistryActivationTester
    {
        [Fact]
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