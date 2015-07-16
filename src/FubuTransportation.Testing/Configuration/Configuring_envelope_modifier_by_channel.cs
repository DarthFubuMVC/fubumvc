using System.Linq;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Runtime;
using NUnit.Framework;

namespace FubuTransportation.Testing.Configuration
{
    [TestFixture]
    public class Configuring_envelope_modifier_by_channel
    {
        [Test]
        public void can_register_modifiers_by_channel()
        {
            using (var runtime = FubuTransport.For<ModifyingChannelRegistry>().Bootstrap())
            {
                var graph = runtime.Factory.Get<ChannelGraph>();

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
            EnableInMemoryTransport();

            Services(x => {
                x.ReplaceService(InMemoryTransport.ToInMemory<BusSettings>());
            });

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