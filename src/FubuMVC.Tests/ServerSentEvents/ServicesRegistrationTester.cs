using FubuMVC.Core;
using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class ServicesRegistrationTester
    {
        [Test]
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