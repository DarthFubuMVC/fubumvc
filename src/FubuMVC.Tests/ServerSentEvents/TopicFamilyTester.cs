using FubuMVC.Core;
using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class TopicFamilyTester
    {
        [Test]
        public void builds_and_caches_by_topic()
        {
            var factory = new DefaultEventQueueFactory<FakeTopic>();
            var family = new TopicFamily<FakeTopic>(factory);

            var topic1 = new FakeTopic{
                Name = "Tom"
            };

            var topic2 = new FakeTopic
            {
                Name = "Todd"
            };

            family.ChannelFor(topic1).ShouldNotBeNull();
            family.ChannelFor(topic1).ShouldBeTheSameAs(family.ChannelFor(topic1));
            family.ChannelFor(topic1).ShouldBeTheSameAs(family.ChannelFor(topic1));
            family.ChannelFor(topic1).ShouldBeTheSameAs(family.ChannelFor(topic1));
            
            family.ChannelFor(topic2).ShouldBeTheSameAs(family.ChannelFor(topic2));


            family.ChannelFor(topic1).ShouldNotBeTheSameAs(family.ChannelFor(topic2));
        }

        [Test]
        public void spin_up_pre_builds()
        {
            var factory = MockRepository.GenerateMock<IEventQueueFactory<FakeTopic>>();
            var queue = new EventQueue<FakeTopic>();
            var topic = new FakeTopic{
                Name = "Top"
            };

            factory.Stub(x => x.BuildFor(topic)).Return(queue);

            var family = new TopicFamily<FakeTopic>(factory);
            family.SpinUpChannel(topic);

            factory.AssertWasCalled(x => x.BuildFor(topic));
        }
    }

    public class DifferentTopic : Topic
    {
        [RouteInput]
        public string Name { get; set; }



        public bool Equals(DifferentTopic other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DifferentTopic)) return false;
            return Equals((DifferentTopic) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }

    public class FakeTopic : Topic
    {
        public string Name { get; set; }

        public bool Equals(FakeTopic other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FakeTopic)) return false;
            return Equals((FakeTopic) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }
}
