using FubuMVC.Core.ServiceBus;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    
    public class TransportSettingsTester
    {
        [Fact]
        public void debug_is_disabled_by_default()
        {
            new TransportSettings().DebugEnabled.ShouldBeFalse();
        }

        [Fact]
        public void the_default_delayed_message_polling_is_5_seconds()
        {
            new TransportSettings().DelayMessagePolling.ShouldBe(5000);
        }

        [Fact]
        public void in_memory_transport_is_disabled_by_default()
        {
            new TransportSettings().InMemoryTransport.ShouldBe(InMemoryTransportMode.Disabled);
        }
    }
}