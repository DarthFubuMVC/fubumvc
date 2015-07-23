using System;
using System.Linq;
using FubuMVC.Core.Services.Messaging;
using FubuMVC.Core.Services.Messaging.Tracking;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Services.Messaging.Tracking
{
    [TestFixture]
    public class MessageHistoryTester
    {
        private IListener<AllMessagesComplete> listener;

        [SetUp]
        public void SetUp()
        {
            MessageHistory.ClearAll();

            listener = MockRepository.GenerateMock<IListener<AllMessagesComplete>>();
            EventAggregator.Messaging.AddListener(listener);

        }

        [TearDown]
        public void TearDown()
        {
            EventAggregator.Messaging.RemoveListener(listener);
        }

        private void assertHasNotReceivedAllCompleteMessage()
        {
            listener.AssertWasNotCalled(x => x.Receive(null), x => x.IgnoreArguments());
        }

        private void assertAllCompleteMessage()
        {
            listener.AssertWasCalled(x => x.Receive(null), x => x.IgnoreArguments());
        }

        [Test]
        public void track_outstanding()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

           MessageHistory.Record(MessageTrack.ForSent(foo1));
           MessageHistory.Record(MessageTrack.ForSent(foo2));
           MessageHistory.Record(MessageTrack.ForSent(foo3));

            MessageHistory.Outstanding().Select(x => x.Id)
                .ShouldHaveTheSameElementsAs(foo1.Id.ToString(), foo2.Id.ToString(), foo3.Id.ToString());

            MessageHistory.Record(MessageTrack.ForReceived(foo2));

            MessageHistory.Outstanding().Select(x => x.Id)
                .ShouldHaveTheSameElementsAs(foo1.Id.ToString(), foo3.Id.ToString());

            MessageHistory.Record(MessageTrack.ForReceived(foo3));

            MessageHistory.Outstanding().Select(x => x.Id)
                .ShouldHaveTheSameElementsAs(foo1.Id.ToString());

            
        }

        [Test]
        public void finish_successfully_with_MessageHistory_WaitForWorkToFinish_positive()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.WaitForWorkToFinish(() => {
                MessageHistory.Record(MessageTrack.ForSent(foo1));
                MessageHistory.Record(MessageTrack.ForSent(foo2));
                MessageHistory.Record(MessageTrack.ForSent(foo3));

                MessageHistory.Record(MessageTrack.ForReceived(foo1));
                MessageHistory.Record(MessageTrack.ForReceived(foo2));
                MessageHistory.Record(MessageTrack.ForReceived(foo3));
            }).ShouldBeTrue();
        }

        [Test]
        public void timeout_unsuccessful_with_MessageHistory_WaitForWorkToFinish_positive()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.WaitForWorkToFinish(() =>
            {
                MessageHistory.Record(MessageTrack.ForSent(foo1));
                MessageHistory.Record(MessageTrack.ForSent(foo2));
                MessageHistory.Record(MessageTrack.ForSent(foo3));

                MessageHistory.Record(MessageTrack.ForReceived(foo1));
                MessageHistory.Record(MessageTrack.ForReceived(foo2));
                //MessageHistory.Record(MessageTrack.ForReceived(foo3));
            }).ShouldBeFalse();
        }

        [Test]
        public void track_received()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.Record(MessageTrack.ForReceived(foo1));
            MessageHistory.Record(MessageTrack.ForReceived(foo2));
            MessageHistory.Record(MessageTrack.ForReceived(foo3));

            MessageHistory.Received().Select(x => x.Id)
                .ShouldHaveTheSameElementsAs(foo1.Id.ToString(), foo2.Id.ToString(), foo3.Id.ToString());

        }

        [Test]
        public void clear_all_absolutely_has_to_work()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.Record(MessageTrack.ForReceived(foo1));
            MessageHistory.Record(MessageTrack.ForReceived(foo2));
            MessageHistory.Record(MessageTrack.ForReceived(foo3));

            MessageHistory.Record(MessageTrack.ForSent(foo1));
            MessageHistory.Record(MessageTrack.ForSent(foo2));
            MessageHistory.Record(MessageTrack.ForSent(foo3));

            MessageHistory.ClearAll();

            MessageHistory.Outstanding().Any().ShouldBeFalse();
            MessageHistory.Received().Any().ShouldBeFalse();
            MessageHistory.All().Any().ShouldBeFalse();
            
        }

        [Test]
        public void sends_the_all_clear_message_when_it_gets_everything()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.Record(MessageTrack.ForSent(foo1));
            MessageHistory.Record(MessageTrack.ForSent(foo2));
            MessageHistory.Record(MessageTrack.ForSent(foo3));

            assertHasNotReceivedAllCompleteMessage();

            MessageHistory.Record(MessageTrack.ForReceived(foo1));
            assertHasNotReceivedAllCompleteMessage();

            MessageHistory.Record(MessageTrack.ForReceived(foo2));
            assertHasNotReceivedAllCompleteMessage();
            
            MessageHistory.Record(MessageTrack.ForReceived(foo3));
            assertAllCompleteMessage();
        }

        [Test]
        public void clear_history_removes_all()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            MessageHistory.Record(MessageTrack.ForReceived(foo1));
            MessageHistory.Record(MessageTrack.ForReceived(foo2));
            MessageHistory.Record(MessageTrack.ForReceived(foo3));

            MessageHistory.Record(MessageTrack.ForSent(foo1));
            MessageHistory.Record(MessageTrack.ForSent(foo2));
            MessageHistory.Record(MessageTrack.ForSent(foo3));

            MessageHistory.ClearHistory();

            MessageHistory.Outstanding().Any().ShouldBeFalse();
            MessageHistory.Received().Any().ShouldBeFalse();
            MessageHistory.All().Any().ShouldBeFalse();
        }
    }

    public class Foo
    {
        public Foo()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}