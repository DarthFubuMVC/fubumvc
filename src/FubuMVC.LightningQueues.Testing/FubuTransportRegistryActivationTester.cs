using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class FubuTransportRegistryActivationTester
    {
        private FubuRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new FubuRegistry();
            //_registry.Services<FubuTransportServiceRegistry>();
        }

        [Test]
        public void should_start_when_transport_disabled()
        {
            _registry.AlterSettings<TransportSettings>(x => {
                                                                x.Enabled = true;
                                                                x.EnableInMemoryTransport = true;
            });
            _registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);
            BootstrapApplication();
        }


        private void BootstrapApplication()
        {
            var application = _registry.ToRuntime();
        }
    }
}