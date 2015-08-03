using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class Configuring_envelope_modifier_by_channel
    {
        [Test]
        public void can_register_modifiers_by_channel()
        {
            using (var runtime = FubuRuntime.For<ModifyingChannelRegistry>())
            {
                var graph = runtime.Get<ChannelGraph>();

                graph.ChannelFor<BusSettings>(x => x.Downstream).Modifiers.Single().ShouldBeOfType<FooModifier>();
                graph.ChannelFor<BusSettings>(x => x.Upstream).Modifiers.Single().ShouldBeOfType<BarModifier>();
                graph.ChannelFor<BusSettings>(x => x.Outbound).Modifiers.Any().ShouldBeFalse();
            }
        }
    }

    public class ModifyingChannelRegistry : FubuTransportRegistry<BusSettings>
    {
        public ModifyingChannelRegistry()
        {
            ServiceBus.Enable(true);
            ServiceBus.EnableInMemoryTransport();

            Services.ReplaceService(InMemoryTransport.ToInMemory<BusSettings>());

            Channel(x => x.Downstream).ModifyWith<FooModifier>();

            Channel(x => x.Upstream).ModifyWith(new BarModifier());

            Channel(x => x.Outbound).ReadIncoming();
        }
    }

    

    public class FooModifier : IEnvelopeModifier
    {
        public void Modify(Envelope envelope)
        {
            
        }
    }

    public class BarModifier : IEnvelopeModifier
    {
        public void Modify(Envelope envelope)
        {
            throw new System.NotImplementedException();
        }
    }
}