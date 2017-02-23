using System.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServerSentEvents;
using Xunit;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServerSentEvents
{

    
    public class Can_build_from_container
    {
        [Fact]
        public void end_to_end()
        {
            var registry = new FubuRegistry();
            registry.Features.ServerSentEvents.Enable(true);

            using (var runtime = registry.ToRuntime())
            {
                var chain = runtime.Behaviors.ChainFor<ChannelWriter<FakeTopic>>(x => x.Write(null));
                var container = runtime.Get<IContainer>();
                container.GetInstance<IChannelInitializer<FakeTopic>>()
                    .ShouldBeOfType<DefaultChannelInitializer<FakeTopic>>();

                container.GetInstance<IActionBehavior>(chain.UniqueId.ToString());
            }

            
        }
    }

    
    public class ServerSentEventExtensionIntegratedTester
    {
        private BehaviorGraph theGraph;

        public ServerSentEventExtensionIntegratedTester()
        {
            var registry = new FubuRegistry();
            registry.Features.ServerSentEvents.Enable(true);

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void should_have_an_endpoint_for_concrete_topic_class()
        {
            theGraph.ChainFor<ChannelWriter<FakeTopic>>(x => x.Write(null))
                .ShouldNotBeNull();

            theGraph.ChainFor<ChannelWriter<DifferentTopic>>(x => x.Write(null))
                .ShouldNotBeNull();
        }

        [Fact]
        public void should_have_url_patter_for_the_topic_classes()
        {
            theGraph.ChainFor<ChannelWriter<FakeTopic>>(x => x.Write(null))
                .As<SseTopicChain>()
                .Route.Pattern.ShouldBe("_events/fake");
        }

        [Fact]
        public void should_have_url_patter_for_the_topic_classes_with_route()
        {
            var route = theGraph
                .ChainFor<ChannelWriter<DifferentTopic>>(x => x.Write(null))
                .As<SseTopicChain>()
                .Route;

            route.Pattern.ShouldBe("_events/different/{Name}");
        }
    }
}