using FubuMVC.Core.Security.Authentication.Saml2.Xml;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    
    public class can_establish_a_control_queue
    {
        [Fact]
        public void configure_a_control_queue()
        {
            var registry = new ControlQueueRegistry();

            using (var runtime = registry.ToRuntime())
            {
                var graph = runtime.Get<ChannelGraph>();

                graph.ControlChannel.Uri.ShouldBe("memory://2".ToUri());

                graph.ChannelFor<BusSettings>(x => x.Upstream)
                    .Incoming.ShouldBeTrue();
            }


        }

        public class ControlQueueRegistry : FubuTransportRegistry<BusSettings>
        {
            public ControlQueueRegistry()
            {
                AlterSettings<BusSettings>(x =>
                {
                    x.Downstream = "memory://1".ToUri();
                    x.Upstream = "memory://2".ToUri();
                });

                Channel(x => x.Downstream).ReadIncoming();
                Channel(x => x.Upstream).UseAsControlChannel();

                ServiceBus.EnableInMemoryTransport();
                

                Mode = "Testing";
            }
        }
    }
}