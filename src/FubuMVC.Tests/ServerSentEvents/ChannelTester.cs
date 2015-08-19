using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class ChannelTester
    {
        private EventQueue<FakeTopic> theQueue;
        private Channel<FakeTopic> theChannel;

        [SetUp]
        public void SetUp()
        {
            theQueue = new EventQueue<FakeTopic>();
            theChannel = new Channel<FakeTopic>(theQueue);
        }

        [Test]
        public void is_connected_at_startup_time()
        {
            theChannel.IsConnected().ShouldBeTrue();
        }

        [Test]
        public void flush_clears_out_the_channel_and_sets_the_connection_to_false()
        {
            theChannel.Flush();
            theChannel.IsConnected().ShouldBeFalse();
        }

        [Test]
        public void multiple_listeners_get_the_events_when_new_events_are_written_to_the_channel()
        {
            var task1 = theChannel.FindEvents(null);
            var task2 = theChannel.FindEvents(null);
            var task3 = theChannel.FindEvents(null);

            var e1 = new ServerEvent("1", "data-1");
            var e2 = new ServerEvent("2", "data-2");
            var e3 = new ServerEvent("3", "data-3");

            theChannel.Write(q =>
            {
                q.Write(e1, e2, e3);
            });

            task1.Result.ShouldHaveTheSameElementsAs(e1, e2, e3);
            task2.Result.ShouldHaveTheSameElementsAs(e1, e2, e3);
            task3.Result.ShouldHaveTheSameElementsAs(e1, e2, e3);
        }

        [Test]
        public void multiple_listeners_at_different_places_in_their_queue()
        {
            var task1 = theChannel.FindEvents(new FakeTopic{LastEventId = "1"});
            var task2 = theChannel.FindEvents(new FakeTopic{LastEventId = "2"});
            var task3 = theChannel.FindEvents(null);

            var e1 = new ServerEvent("1", "data-1");
            var e2 = new ServerEvent("2", "data-2");
            var e3 = new ServerEvent("3", "data-3");

            theChannel.Write(q =>
            {
                q.Write(e1, e2, e3);
            });

            task1.Result.ShouldHaveTheSameElementsAs(e2, e3);
            task2.Result.ShouldHaveTheSameElementsAs(e3);
            task3.Result.ShouldHaveTheSameElementsAs(e1, e2, e3);
        }

        [Test]
        public void find_events_that_are_already_published()
        {
            var e1 = new ServerEvent("1", "data-1");
            var e2 = new ServerEvent("2", "data-2");
            var e3 = new ServerEvent("3", "data-3");

            theChannel.Write(q =>
            {
                q.Write(e1, e2, e3);
            });

            var task1 = theChannel.FindEvents(new FakeTopic { LastEventId = "1" });
            var task2 = theChannel.FindEvents(new FakeTopic { LastEventId = "2" });
            var task3 = theChannel.FindEvents(null);

            task1.Result.ShouldHaveTheSameElementsAs(e2, e3);
            task2.Result.ShouldHaveTheSameElementsAs(e3);
            task3.Result.ShouldHaveTheSameElementsAs(e1, e2, e3);
        }

        [Test]
        public void disconnected_channel_returns_empty_enumerable()
        {
            var e1 = new ServerEvent("1", "data-1");
            var e2 = new ServerEvent("2", "data-2");
            var e3 = new ServerEvent("3", "data-3");

            theChannel.Write(q =>
            {
                q.Write(e1, e2, e3);
            });

            theChannel.Flush();
            theChannel.IsConnected().ShouldBeFalse();

            var task = theChannel.FindEvents(null);

            task.Wait(150).ShouldBeTrue();

            task.Result.ShouldHaveCount(0);
        }
    }
}