using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class TransportSettingsTester
    {
        [Test]
        public void debug_is_disabled_by_default()
        {
            new TransportSettings().DebugEnabled.ShouldBeFalse();
        }

        [Test]
        public void the_default_delayed_message_polling_is_5_seconds()
        {
            new TransportSettings().DelayMessagePolling.ShouldBe(5000);
        }

        [Test]
        public void in_memory_transport_is_disabled_by_default()
        {
            new TransportSettings().InMemoryTransport.ShouldBe(InMemoryTransportMode.Disabled);
        }
    }
}