using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Tests.ServiceBus;
using Xunit;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class throws_descriptive_message_if_a_single_port_has_mixed_modes
    {
        [Fact]
        public void throws()
        {
            var registry = new SampleRegistry();

            var outer = Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                registry.ToRuntime();
            });

            
            var expectedText = "The LightningQueues listener for port 2222 has\r\n mixed channel modes. Either make the modes be the same or use a different port number";

            outer.ToString().Contains(expectedText);
        }

        public class SampleRegistry : FubuTransportRegistry<BusSettings>
        {
            public SampleRegistry()
            {
                AlterSettings<BusSettings>(x =>
                {
                    x.Downstream = "lq.tcp://localhost:2222/downstream".ToUri();
                    x.Outbound = "lq.tcp://localhost:2222/outbound".ToUri();
                });

                Channel(x => x.Downstream).DeliveryGuaranteed();
                Channel(x => x.Outbound).DeliveryFastWithoutGuarantee();
            }
        }
    }
}