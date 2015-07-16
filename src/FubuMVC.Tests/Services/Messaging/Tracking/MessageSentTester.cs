using System;
using FubuMVC.Core.Services.Messaging.Tracking;
using FubuTestingSupport;
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

            sent.FullName.ShouldEqual(typeof (MyMessage).FullName);
            sent.Type.ShouldEqual(typeof (MyMessage).Name);
            sent.Description.ShouldEqual(message.ToString());
            sent.Id.ShouldEqual(message.Id.ToString());
            sent.Status.ShouldEqual(MessageTrack.Sent);
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

            sent.FullName.ShouldEqual(typeof(MyMessage).FullName);
            sent.Type.ShouldEqual(typeof(MyMessage).Name);
            sent.Description.ShouldEqual(message.ToString());
            sent.Id.ShouldEqual("foo");
            sent.Status.ShouldEqual(MessageTrack.Sent);
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

            sent.FullName.ShouldEqual(typeof(MyMessage).FullName);
            sent.Type.ShouldEqual(typeof(MyMessage).Name);
            sent.Description.ShouldEqual(message.ToString());
            sent.Id.ShouldEqual(message.Id.ToString());
            sent.Status.ShouldEqual(MessageTrack.Received);
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

            sent.FullName.ShouldEqual(typeof(MyMessage).FullName);
            sent.Type.ShouldEqual(typeof(MyMessage).Name);
            sent.Description.ShouldEqual(message.ToString());
            sent.Id.ShouldEqual("bar");
            sent.Status.ShouldEqual(MessageTrack.Received);
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