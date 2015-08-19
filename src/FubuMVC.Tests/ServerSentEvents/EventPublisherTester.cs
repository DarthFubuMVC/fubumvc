using FubuMVC.Core.ServerSentEvents;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class EventPublisherTester : InteractionContext<EventPublisher>
    {
        [Test]
        public void publish_a_server_event()
        {
            var @event = new ServerEvent("1", "some text");
            var topic = new FakeTopic{
                Name = "something"
            };

            var theChannel = MockFor<ITopicChannel<FakeTopic>>();
            MockFor<ITopicChannelCache>().Stub(x => x.ChannelFor(topic)).Return(theChannel);

            ClassUnderTest.WriteTo(topic, @event);

            theChannel.AssertWasCalled(x => x.WriteEvents(topic, @event));
        }

        [Test]
        public void publish_against_a_different_interface()
        {
            var oddQueue = MockFor<IOddQueue>();
            var @event = new ServerEvent("1", "some text");
            var topic = new FakeTopic
            {
                Name = "something"
            };

            var theChannel = new TopicChannel<FakeTopic>(oddQueue);
            MockFor<ITopicChannelCache>().Stub(x => x.ChannelFor(topic)).Return(theChannel);

            ClassUnderTest.WriteTo<FakeTopic, IOddQueue>(topic, x =>
            {
                x.WriteOddly(@event);
            });

            oddQueue.AssertWasCalled(x => x.WriteOddly(@event));
        }

        public interface IOddQueue : IEventQueue<FakeTopic>
        {
            void WriteOddly(IServerEvent @event);
        }
    }
}