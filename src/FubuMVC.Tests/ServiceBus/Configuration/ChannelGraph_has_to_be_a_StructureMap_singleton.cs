using FubuMVC.Core.ServiceBus.Configuration;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class ChannelGraph_has_to_be_a_StructureMap_singleton
    {
        [Test]
        public void must_be_a_singleton()
        {
            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap()
                )
            {
                var graph1 = runtime.Container.GetInstance<ChannelGraph>();
                var graph2 = runtime.Container.GetInstance<ChannelGraph>();
                var graph3 = runtime.Container.GetInstance<ChannelGraph>();
                var graph4 = runtime.Container.GetInstance<ChannelGraph>();

                graph1.ShouldBeTheSameAs(graph2);
                graph1.ShouldBeTheSameAs(graph3);
                graph1.ShouldBeTheSameAs(graph4);
            }

           
        }
    }
}