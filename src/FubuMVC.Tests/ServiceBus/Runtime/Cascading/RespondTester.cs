using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Cascading
{
    [TestFixture]
    public class RespondTester
    {
        private Message1 theMessage;
        private Envelope theOriginalEnvelope;

        [SetUp]
        public void SetUp()
        {
            theMessage = new Message1();
            theOriginalEnvelope = new Envelope
            {
                ReplyUri = "lq://foo".ToUri()
            };
        }

        [Test]
        public void respond_gets_the_message_on_the_new_envelope()
        {
            Respond.With(theMessage).CreateEnvelope(theOriginalEnvelope)
                .Message.ShouldBeTheSameAs(theMessage);
        }

        [Test]
        public void with_headers()
        {
            var response = Respond.With(theMessage).WithHeader("A", "1").WithHeader("B", "2");
            var envelope = response
                .CreateEnvelope(theOriginalEnvelope);

            envelope.Headers["A"].ShouldBe("1");
            envelope.Headers["B"].ShouldBe("2");
        
            response.ToString().ShouldContain("A='1'");
            response.ToString().ShouldContain("B='2'");
        }

        [Test]
        public void to_sender()
        {
            var response = Respond.With(theMessage).ToSender();
            var envelope = response.CreateEnvelope(theOriginalEnvelope);

            envelope.Destination.ShouldBe(theOriginalEnvelope.ReplyUri);

            response.ToString().ShouldContain("respond to sender");
        }

        [Test]
        public void to_destination()
        {
            var uri = "lq://bar".ToUri();

            var response = Respond.With(theMessage).To(uri);
            var envelope = response.CreateEnvelope(theOriginalEnvelope);

            envelope.Destination.ShouldBe(uri);

            response.ToString().ShouldContain("Destination=" + uri);
        }

        [Test]
        public void delayed_until()
        {
            var time = DateTime.Today.ToUniversalTime();
            var response = Respond.With(theMessage).DelayedUntil(time);
            var envelope = response.CreateEnvelope(theOriginalEnvelope);

            envelope.ExecutionTime.ShouldBe(time);


        }

        [Test]
        public void delay_by()
        {
            var now = DateTime.UtcNow;

            var response = Respond.With(theMessage).DelayedBy(5.Minutes());
            var envelope = response.CreateEnvelope(theOriginalEnvelope);

            // Fuzzy match and I'm being lazy.  You can't trust equals on DateTime
            // and we're not faking the clock
            (envelope.ExecutionTime > now.AddMinutes(4)).ShouldBeTrue();
            (envelope.ExecutionTime < now.AddMinutes(6)).ShouldBeTrue();
        }

        [Test]
        public void assert_was_sent_back_to_sender_happy_path()
        {
            Respond.With(new Message1()).ToSender()
                .AssertWasSentBackToSender();
        }

        [Test]
        public void assert_was_sent_back_to_sender_sad_path()
        {
            Exception<Exception>.ShouldBeThrownBy(() => {
                Respond.With(new Message1())
                    .AssertWasSentBackToSender();
            }).Message.ShouldContain("Was NOT sent back to the sender");
        }
    }

    
}