using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using Xunit;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    
    public class ChannelGraph_has_to_be_a_StructureMap_singleton
    {
        [Fact]
        public void must_be_a_singleton()
        {
            using (var runtime = FubuRuntime.BasicBus()
                )
            {
                var container = runtime.Get<IContainer>();

                var graph1 = container.GetInstance<ChannelGraph>();
                var graph2 = container.GetInstance<ChannelGraph>();
                var graph3 = container.GetInstance<ChannelGraph>();
                var graph4 = container.GetInstance<ChannelGraph>();

                graph1.ShouldBeTheSameAs(graph2);
                graph1.ShouldBeTheSameAs(graph3);
                graph1.ShouldBeTheSameAs(graph4);
            }

           
        }
    }
}