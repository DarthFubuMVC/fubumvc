using System.Linq;
using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class SimpleEventQueueTester
    {
        private EventQueue<FakeTopic> theQueue;

        [SetUp]
        public void SetUp()
        {
            theQueue = new EventQueue<FakeTopic>();
            theQueue.Write(new ServerEvent("2", "2"));
            theQueue.Write(new ServerEvent("1", "1"));
            
            for (int i = 3; i < 8; i++)
            {
                theQueue.Write(new ServerEvent(i.ToString(), i.ToString()));
            }    
        }

        [Test]
        public void when_the_topic_has_no_last_event_return_everything_in_order()
        {
            theQueue.FindQueuedEvents(new FakeTopic{LastEventId = null}).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("2", "1", "3", "4", "5", "6", "7");

            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = string.Empty }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("2", "1", "3", "4", "5", "6", "7");
        }

        [Test]
        public void return_nothing_if_the_last_event_id_is_current()
        {
            theQueue.FindQueuedEvents(new FakeTopic(){LastEventId = "7"})
                .Any().ShouldBeFalse();
        }

        [Test]
        public void return_everything_if_the_event_id_does_not_match()
        {
            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = "random" }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("2", "1", "3", "4", "5", "6", "7");
        }

        [Test]
        public void return_everything_if_the_topic_is_null()
        {
            theQueue.FindQueuedEvents(null).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("2", "1", "3", "4", "5", "6", "7");
        }

        [Test]
        public void return_nothing_if_there_are_no_events()
        {
            theQueue.AllEvents.Clear();

            theQueue.FindQueuedEvents(new FakeTopic() { LastEventId = null })
                .Any().ShouldBeFalse();
        }

        [Test]
        public void return_everything_following_if_the_last_topic_matches()
        {
            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = "1" }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("3", "4", "5", "6", "7");

            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = "3" }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("4", "5", "6", "7");

            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = "4" }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("5", "6", "7");

            theQueue.FindQueuedEvents(new FakeTopic { LastEventId = "6" }).Select(x => x.Id)
                .ShouldHaveTheSameElementsAs("7");
        }


    }
}