using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    
    public class RecordingServiceBusTester
    {
        [Fact]
        public void send_records()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Send(message);

            bus.Sent.Single().ShouldBeTheSameAs(message);
        }

        [Fact]
        public void consumed_messages()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Consume(message);

            bus.Consumed.Single().ShouldBeTheSameAs(message);
        }

        [Fact]
        public void send_delayed_by_time_span()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.DelaySend(message, 5.Minutes());

            bus.DelayedSent.Single().Message.ShouldBeTheSameAs(message);
            bus.DelayedSent.Single().Delay.ShouldBe(5.Minutes());
        }

        [Fact]
        public void send_delayed_by_time()
        {
            var time = DateTime.Today.AddHours(5);

            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.DelaySend(message, time);

            bus.DelayedSent.Single().Message.ShouldBeTheSameAs(message);
            bus.DelayedSent.Single().Time.ShouldBe(time);

        }

        [Fact]
        public void send_and_wait()
        {
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.SendAndWait(message).Wait();

            // Checking for messages sent
            bus.Sent.Single().ShouldBeTheSameAs(message);

            // Checking for awaited calls
            bus.Await.Single().ShouldBeTheSameAs(message);
        }

        [Fact]
        public void send_to_destination()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.Send(destination, message);

            var sentTo = bus.SentDirectlyTo.Single();
            sentTo.Destination.ShouldBe(destination);
            sentTo.Message.ShouldBeTheSameAs(message);
        }

        [Fact]
        public void send_to_destination_and_wait()
        {
            var destination = new Uri("memory://blah");
            var message = new Message1();
            var bus = new RecordingServiceBus();

            bus.SendAndWait(destination, message);

            var sentTo = bus.SentDirectlyTo.Single();
            sentTo.Destination.ShouldBe(destination);
            sentTo.Message.ShouldBeTheSameAs(message);
        }
    }
}