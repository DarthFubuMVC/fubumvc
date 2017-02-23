using FubuMVC.Core.ServerSentEvents;
using FubuMVC.Core.StructureMap;
using Xunit;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServerSentEvents
{
    
    public class EventQueryFactoryTester
    {
        private IContainer _container;
        private EventQueueFactory _factory;

        public EventQueryFactoryTester()
        {
            _container = new Container(x => x.For(typeof(IEventQueueFactory<>)).Use(typeof(DefaultEventQueueFactory<>)));
            var services = new StructureMapServiceLocator(_container);
            _factory = new EventQueueFactory(services);
        }

        [Fact]
        public void build_for_returns_a_simple_queue()
        {
            _factory.BuildFor(new FakeTopic()).ShouldBeOfType<EventQueue<FakeTopic>>();
            _factory.BuildFor(new DifferentTopic()).ShouldBeOfType<EventQueue<DifferentTopic>>();
            _factory.BuildFor(new DifferentTopic()).ShouldNotBeOfType<AlternateEventQueue<DifferentTopic>>();
        }

        [Fact]
        public void build_for_returns_alternate_queue()
        {
            _container.Configure(x => x.For<IEventQueueFactory<DifferentTopic>>().Use<AlternateEventQueueFactory>());

            _factory.BuildFor(new FakeTopic()).ShouldBeOfType<EventQueue<FakeTopic>>();
            _factory.BuildFor(new DifferentTopic()).ShouldBeOfType<AlternateEventQueue<DifferentTopic>>();
        }
    }

    public class AlternateEventQueueFactory : IEventQueueFactory<DifferentTopic>
    {
        public IEventQueue<DifferentTopic> BuildFor(DifferentTopic topic)
        {
            return new AlternateEventQueue<DifferentTopic>();
        }
    }

    public class AlternateEventQueue<T> : EventQueue<T> where T : Topic { }
}