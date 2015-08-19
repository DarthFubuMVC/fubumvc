using System;
using FubuCore;
using FubuMVC.Core.ServerSentEvents;
using FubuMVC.Core.StructureMap;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class TopicChannelCacheTester : InteractionContext<TopicChannelCache>
    {
        private FakeAspNetShutDownDetector _shutdownDetector;

        [SetUp]
        protected override void  beforeEach()
        {
            _shutdownDetector = new FakeAspNetShutDownDetector();

            Container.Configure(x =>
            {
                x.For(typeof (IEventQueueFactory<>)).Use(typeof (DefaultEventQueueFactory<>));
                x.For<IServiceLocator>().Use<StructureMapServiceLocator>();
                x.For<IAspNetShutDownDetector>().Use(_shutdownDetector);
            });
        }

        [Test]
        public void caches_against_a_single_topic_family()
        {
            var topic1 = new FakeTopic
            {
                Name = "Tom"
            };

            var topic2 = new FakeTopic
            {
                Name = "Todd"
            };

            ClassUnderTest.ChannelFor(topic1).ShouldNotBeNull();
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));

            ClassUnderTest.ChannelFor(topic2).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic2));

            ClassUnderTest.ChannelFor(topic1).ShouldNotBeTheSameAs(ClassUnderTest.ChannelFor(topic2));
        }

        [Test]
        public void caches_against_a_multiple_topic_families()
        {
            // All relatives of mine from the same family.  In-law finally
            // rebelled and named a child "Parker"
            var topic1 = new FakeTopic
            {
                Name = "Tom"
            };

            var topic2 = new FakeTopic
            {
                Name = "Todd"
            };

            var topic3 = new DifferentTopic{
                Name = "Trevor"
            };

            var topic4 = new DifferentTopic
            {
                Name = "Trent"
            };

            ClassUnderTest.ChannelFor(topic1).ShouldNotBeNull();
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));
            ClassUnderTest.ChannelFor(topic1).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic1));

            ClassUnderTest.ChannelFor(topic2).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic2));

            ClassUnderTest.ChannelFor(topic1).ShouldNotBeTheSameAs(ClassUnderTest.ChannelFor(topic2));

            ClassUnderTest.ChannelFor(topic3).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic3));

            ClassUnderTest.ChannelFor(topic4).ShouldBeTheSameAs(ClassUnderTest.ChannelFor(topic4));

            ClassUnderTest.ChannelFor(topic3).ShouldNotBeTheSameAs(ClassUnderTest.ChannelFor(topic4));
        }

        [Test]
        public void disposes_on_asp_shutdown_event()
        {
            var topic1 = new FakeTopic
            {
                Name = "Tom"
            };

            var topic2 = new FakeTopic
            {
                Name = "Todd"
            };

            var channel1 = ClassUnderTest.ChannelFor(topic1);
            var channel2 = ClassUnderTest.ChannelFor(topic2);

            channel1.ShouldNotBeNull();
            channel2.ShouldNotBeNull();

            _shutdownDetector.Stop(true);

            channel1.Channel.IsConnected().ShouldBeFalse();
            channel2.Channel.IsConnected().ShouldBeFalse();

            Exception<ObjectDisposedException>.ShouldBeThrownBy(() => ClassUnderTest.ChannelFor(topic1));
            Exception<ObjectDisposedException>.ShouldBeThrownBy(() => ClassUnderTest.ChannelFor(topic2));
        }
    }

    public class FakeAspNetShutDownDetector : IAspNetShutDownDetector
    {
        private Action _onShutdown;

        public void Stop(bool immediate)
        {
            _onShutdown();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Register(Action onShutdown)
        {
            _onShutdown = onShutdown;
        }
    }
}