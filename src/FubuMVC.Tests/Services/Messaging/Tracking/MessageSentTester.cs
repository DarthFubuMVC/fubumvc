using System;
using FubuMVC.Core.Services.Messaging.Tracking;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Messaging.Tracking
{
    [TestFixture]
    public class MessageSentTester
    {
        [Test]
        public void record_sent_message()
        {
            var message = new MyMessage
            {
                Id = Guid.NewGuid(),
                Name = "Jeremy"
            };

            var sent = MessageTrack.ForSent(message);

            sent.FullName.ShouldBe(typeof (MyMessage).FullName);
            sent.Type.ShouldBe(typeof (MyMessage).Name);
            sent.Description.ShouldBe(message.ToString());
            sent.Id.ShouldBe(message.Id.ToString());
            sent.Status.ShouldBe(MessageTrack.Sent);
        }

        [Test]
        public void record_sent_message_with_id()
        {
            var message = new MyMessage
            {
                Id = Guid.NewGuid(),
                Name = "Jeremy"
            };

            var sent = MessageTrack.ForSent(message, "foo");

            sent.FullName.ShouldBe(typeof(MyMessage).FullName);
            sent.Type.ShouldBe(typeof(MyMessage).Name);
            sent.Description.ShouldBe(message.ToString());
            sent.Id.ShouldBe("foo");
            sent.Status.ShouldBe(MessageTrack.Sent);
        }

        [Test]
        public void record_received_message()
        {
            var message = new MyMessage
            {
                Id = Guid.NewGuid(),
                Name = "Jeremy"
            };

            var sent = MessageTrack.ForReceived(message);

            sent.FullName.ShouldBe(typeof(MyMessage).FullName);
            sent.Type.ShouldBe(typeof(MyMessage).Name);
            sent.Description.ShouldBe(message.ToString());
            sent.Id.ShouldBe(message.Id.ToString());
            sent.Status.ShouldBe(MessageTrack.Received);
        }

        [Test]
        public void record_received_message_with_explicit_id()
        {
            var message = new MyMessage
            {
                Id = Guid.NewGuid(),
                Name = "Jeremy"
            };

            var sent = MessageTrack.ForReceived(message, "bar");

            sent.FullName.ShouldBe(typeof(MyMessage).FullName);
            sent.Type.ShouldBe(typeof(MyMessage).Name);
            sent.Description.ShouldBe(message.ToString());
            sent.Id.ShouldBe("bar");
            sent.Status.ShouldBe(MessageTrack.Received);
        }
    }

    public class MyMessage
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }
}