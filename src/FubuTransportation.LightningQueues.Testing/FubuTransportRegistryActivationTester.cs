using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class FubuTransportRegistryActivationTester
    {
        private FubuRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _registry = new FubuRegistry();
            _registry.Services<FubuTransportServiceRegistry>();
        }

        [Test]
        public void should_start_when_transport_disabled()
        {
            _registry.AlterSettings<TransportSettings>(x => x.Disabled = true);
            _registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);
            BootstrapApplication();
        }

        [Test]
        public void should_throw_when_no_transports_are_registered()
        {
            Assert.Throws<FubuException>(BootstrapApplication,
                "No transports are registered.  FubuTransportation cannot function without at least one ITransport");
        }

        private void BootstrapApplication()
        {
            var application = FubuApplication.For(_registry).StructureMap();
            application.Bootstrap();
        }
    }
}