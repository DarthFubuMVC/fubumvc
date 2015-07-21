using System.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuTransportation.Testing.Configuration
{
    [TestFixture]
    public class FubuTransportRegistryTester
    {
        [Test]
        public void able_to_derive_the_node_name_from_fubu_transport_registry_name()
        {
            using (var runtime = FubuTransport.For<CustomTransportRegistry>().Bootstrap())
            {
                runtime.Factory.Get<ChannelGraph>().Name.ShouldEqual("custom");
            }

            using (var fubuRuntime = FubuTransport.For<OtherRegistry>().Bootstrap())
            {
                fubuRuntime
                    .Factory.Get<ChannelGraph>().Name.ShouldEqual("other");
            }
        }

        [Test]
        public void can_set_the_node_name_programmatically()
        {
            using (var fubuRuntime = FubuTransport.For(x => {
                x.NodeName = "MyNode";
                x.EnableInMemoryTransport();
            }).Bootstrap())
            {
                fubuRuntime
                    .Factory.Get<ChannelGraph>().Name.ShouldEqual("MyNode");
            }
        }
    }

    public class CustomTransportRegistry : FubuTransportRegistry
    {
        public CustomTransportRegistry()
        {
            EnableInMemoryTransport();
        }
    }

    public class OtherRegistry : FubuTransportRegistry
    {
        public OtherRegistry()
        {
            EnableInMemoryTransport();
        }
    }
}