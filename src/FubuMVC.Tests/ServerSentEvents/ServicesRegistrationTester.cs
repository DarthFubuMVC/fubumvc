using FubuMVC.Core;
using FubuMVC.Core.ServerSentEvents;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.ServerSentEvents
{
    
    public class ServicesRegistrationTester
    {
        [Fact]
        public void register_services()
        {
            var registry = new FubuRegistry();
            registry.Features.ServerSentEvents.Enable(true);

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                container.DefaultRegistrationIs<IEventPublisher, EventPublisher>();
                container.DefaultRegistrationIs<IServerEventWriter, ServerEventWriter>();
                container.DefaultRegistrationIs(typeof (IEventQueueFactory<>), typeof (DefaultEventQueueFactory<>));

                container.DefaultSingletonIs<ITopicChannelCache, TopicChannelCache>();
            }
        }

    }
}