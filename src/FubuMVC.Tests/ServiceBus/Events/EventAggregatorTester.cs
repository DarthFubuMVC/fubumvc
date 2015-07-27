using System;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Events
{
    [TestFixture]
    public class EventAggregatorTester
    {
        private EventAggregator events;
        private StubMessage1Handler handler;
        private RecordingLogger recordingLogger;

        [SetUp]
        public void SetUp()
        {
            recordingLogger = new RecordingLogger();

            events = new EventAggregator(() => recordingLogger, new IListener[0]);

            handler = new StubMessage1Handler();
            events.AddListener(handler);
        }

        [Test]
        public void simple_handlers_registered()
        {
            var theMessage = new Message1();
            events.SendMessage(theMessage);

            Wait.Until(() => handler.Message != null);

            handler.Message.ShouldBeTheSameAs(theMessage);
        }

        [Test]
        public void send_message_to_all_listeners_for_a_message_type()
        {
            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            events.AddListeners(listener1, listener2, listener3, this, listener4);

            var message1 = new Message1();
            var message2 = new Message2();

            events.SendMessage(message1);
            events.SendMessage(message2);

            Wait.Until(
                () =>
                    listener1.LastMessage != null && listener2.LastMessage != null && listener3.LastMessage != null &&
                    listener4.LastMessage != null);

            listener1.LastMessage.ShouldBeTheSameAs(message1);
            listener2.LastMessage.ShouldBeTheSameAs(message1);
            listener3.LastMessage.ShouldBeTheSameAs(message1);

            listener4.LastMessage.ShouldBeTheSameAs(message2);
        }

        [Test]
        public void listeners_can_fail_one_at_a_time()
        {
            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            events.AddListeners(listener1, listener2, listener3, this, listener4);
            events.AddListener(new ErrorCausingHandler());

            var message1 = new Message1();
            var message2 = new Message2();

            events.SendMessage(message1);
            events.SendMessage(message2);

            Wait.Until(
                () =>
                    listener1.LastMessage != null && listener2.LastMessage != null && listener3.LastMessage != null &&
                    listener4.LastMessage != null);

            listener1.LastMessage.ShouldBeTheSameAs(message1);
            listener2.LastMessage.ShouldBeTheSameAs(message1);
            listener3.LastMessage.ShouldBeTheSameAs(message1);

            listener4.LastMessage.ShouldBeTheSameAs(message2);
        }

        [Test]
        public void exposes_all_the_listeners()
        {
            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            events.AddListeners(listener1, listener2, listener3, this, listener4);

            events.Listeners.ShouldHaveTheSameElementsAs(handler, listener1, listener2, listener3, this, listener4);
        }

        [Test]
        public void send_message_that_creates_on_the_fly()
        {
            var listener1 = new StubListener<Message1>();
            events.AddListener(listener1);

            events.SendMessage<Message1>();

            Wait.Until(() => listener1.LastMessage != null);


            listener1.LastMessage.ShouldBeOfType<Message1>();
        }

        [Test]
        public void remove_listener()
        {
            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            var listener5 = MockRepository.GenerateMock<IListener<Message1>>();
            events.AddListeners(listener1, listener2, listener3, this, listener4, listener5);

            var message1 = new Message1();


            events.RemoveListener(listener5);
            events.SendMessage(message1);

            listener5.AssertWasNotCalled(x => x.Handle(message1));
        }

        [Test]
        public void prune_expired_listeners_based_on_the_expired_property()
        {
            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            var listener5 = new ExpiringListener {IsExpired = true};
            var listener6 = new ExpiringListener {IsExpired = true};
            var listener7 = new ExpiringListener {IsExpired = false};

            events.AddListeners(listener1, listener2, listener3, listener4, listener5, listener6, listener7);

            events.PruneExpiredListeners(DateTime.Now);

            // non-expiring listeners should be untouched
            events.Listeners.ShouldContain(listener1);
            events.Listeners.ShouldContain(listener2);
            events.Listeners.ShouldContain(listener3);
            events.Listeners.ShouldContain(listener4);

            // expired expiring listeners should be removed
            events.Listeners.ShouldNotContain(listener5);
            events.Listeners.ShouldNotContain(listener6);

            // not-expired expiring listener should not be removed
            events.Listeners.ShouldContain(listener7);
        }

        [Test]
        public void prune_expired_listeners_based_on_time()
        {
            var now = DateTime.Today.AddHours(5);

            var listener1 = new StubListener<Message1>();
            var listener2 = new StubListener<Message1>();
            var listener3 = new StubListener<Message1>();
            var listener4 = new StubListener<Message2>();

            var listener5 = new ExpiringListener {ExpiresAt = now.AddMinutes(-1)};
            var listener6 = new ExpiringListener {ExpiresAt = now.AddMinutes(-1)};
            var listener7 = new ExpiringListener {ExpiresAt = now.AddMinutes(1)};

            events.AddListeners(listener1, listener2, listener3, listener4, listener5, listener6, listener7);

            events.PruneExpiredListeners(now);

            // non-expiring listeners should be untouched
            events.Listeners.ShouldContain(listener1);
            events.Listeners.ShouldContain(listener2);
            events.Listeners.ShouldContain(listener3);
            events.Listeners.ShouldContain(listener4);

            // expired expiring listeners should be removed
            events.Listeners.ShouldNotContain(listener5);
            events.Listeners.ShouldNotContain(listener6);

            // not-expired expiring listener should not be removed

            listener7.IsExpired(now).ShouldBeFalse();
            events.Listeners.ShouldContain(listener7);
        }
    }
}