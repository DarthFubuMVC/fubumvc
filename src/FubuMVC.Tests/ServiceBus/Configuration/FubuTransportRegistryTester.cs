using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    
    public class FubuTransportRegistryTester
    {
        [Fact]
        public void able_to_derive_the_node_name_from_fubu_transport_registry_name()
        {
            using (var runtime = FubuRuntime.For<CustomTransportRegistry>())
            {
                runtime.Get<ChannelGraph>().Name.ShouldBe("custom");
            }

            using (var fubuRuntime = FubuRuntime.For<OtherRegistry>())
            {
                fubuRuntime.Get<ChannelGraph>().Name.ShouldBe("other");
            }
        }

        [Fact]
        public void can_set_the_node_name_programmatically()
        {
            var registry = new FubuRegistry {NodeName = "MyNode"};


            using (var fubuRuntime = registry.ToRuntime())
            {
                fubuRuntime.Get<ChannelGraph>().Name.ShouldBe("MyNode");
            }
        }
    }

    public class CustomTransportRegistry : FubuRegistry
    {
        public CustomTransportRegistry()
        {
            ServiceBus.Enable(true);
            ServiceBus.EnableInMemoryTransport();
        }
    }

    public class OtherRegistry : FubuRegistry
    {
        public OtherRegistry()
        {
            ServiceBus.Enable(true);
            ServiceBus.EnableInMemoryTransport();
        }
    }
}