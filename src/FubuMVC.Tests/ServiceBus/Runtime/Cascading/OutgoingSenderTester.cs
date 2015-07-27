using System;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Cascading
{
    [TestFixture]
    public class OutgoingSenderTester : InteractionContext<OutgoingSender>
    {
        [Test]
        public void swallows_and_logs_exceptions_on_send()
        {
            // ONLY FOR NOW
            var ex = new NotImplementedException();
            var envelope = new Envelope();

            MockFor<IEnvelopeSender>().Stub(x => x.Send(envelope)).Throw(ex);

            ClassUnderTest.Send(envelope);

            MockFor<ILogger>().AssertWasCalled(x => x.Error(envelope.OriginalId, "Failure while trying to send a cascading message", ex));
        }

        [Test]
        public void use_envelope_from_the_original_if_not_ISendMyself()
        {
            var original = MockRepository.GenerateMock<Envelope>();
            var message = new Message1();

            var resulting = new Envelope();

            original.Expect(x => x.ForResponse(message)).Return(resulting);

            ClassUnderTest.SendOutgoingMessage(original, message);

            MockFor<IEnvelopeSender>().AssertWasCalled(x => x.Send(resulting));
        }

        [Test]
        public void use_envelope_from_ISendMySelf()
        {
            var message = MockRepository.GenerateMock<ISendMyself>();
            var original = new Envelope();
            var resulting = new Envelope();

            message.Stub(x => x.CreateEnvelope(original)).Return(resulting);

            ClassUnderTest.SendOutgoingMessage(original, message);

            MockFor<IEnvelopeSender>().AssertWasCalled(x => x.Send(resulting));
        }

        [Test]
        public void if_original_envelope_is_ack_requested_send_ack_back()
        {
            var original = new Envelope
            {
                ReplyUri = "foo://bar".ToUri(),
                AckRequested = true,
                CorrelationId = Guid.NewGuid().ToString()
            };

            ClassUnderTest.SendOutgoingMessages(original, new object[0]);

            var envelope = MockFor<IEnvelopeSender>().GetArgumentsForCallsMadeOn(x => x.Send(null))[0][0]
                .As<Envelope>();
            envelope.ShouldNotBeNull();

            envelope.ResponseId.ShouldBe(original.CorrelationId);
            envelope.Destination.ShouldBe(original.ReplyUri);
            envelope.Message.ShouldBe(new Acknowledgement {CorrelationId = original.CorrelationId});
            envelope.ParentId.ShouldBe(original.CorrelationId);
        }

        [Test]
        public void do_not_send_ack_if_no_ack_is_requested()
        {
            var original = new Envelope
            {
                ReplyUri = "foo://bar".ToUri(),
                AckRequested = false,
                CorrelationId = Guid.NewGuid().ToString()
            };

            ClassUnderTest.SendOutgoingMessages(original, new object[0]);

            MockFor<IEnvelopeSender>().AssertWasNotCalled(x => x.Send(null), x => x.IgnoreArguments());
        }

        [Test]
        public void when_sending_a_failure_ack_if_no_ack_or_response_is_requested_do_nothing()
        {
            var original = new Envelope
            {
                ReplyUri = "foo://bar".ToUri(),
                AckRequested = false,
                ReplyRequested = null,
                CorrelationId = Guid.NewGuid().ToString()
            };

            var recordingSender = new RecordingEnvelopeSender();
            new OutgoingSender(recordingSender, new RecordingLogger())
                .SendFailureAcknowledgement(original, "you stink");

            recordingSender.Outgoing.Any()
                .ShouldBeFalse();
        }


    }

    [TestFixture]
    public class when_sending_a_failure_ack_and_ack_is_requested
    {
        private FailureAcknowledgement theAck;
        private Envelope theSentEnvelope;
        private Envelope original;

        [SetUp]
        public void SetUp()
        {
            original = new Envelope
            {
                ReplyUri = "foo://bar".ToUri(),
                AckRequested = true,
                CorrelationId = Guid.NewGuid().ToString()
            };

            var recordingSender = new RecordingEnvelopeSender();
            new OutgoingSender(recordingSender, new RecordingLogger())
                .SendFailureAcknowledgement(original, "you stink");

            theSentEnvelope = recordingSender.Sent.Single();
            theAck = theSentEnvelope.Message as FailureAcknowledgement;
        }

        [Test]
        public void should_have_sent_a_failure_ack()
        {
            theAck.ShouldNotBeNull();
        }

        [Test]
        public void the_message_should_be_what_was_requested()
        {
            theAck.Message.ShouldBe("you stink");
        }

        [Test]
        public void should_have_The_parent_id_set_to_the_original_id_for_tracking()
        {
            theSentEnvelope.ParentId.ShouldBe(original.CorrelationId);
        }

        [Test]
        public void should_have_The_correlation_id_from_the_original_envelope()
        {
            theAck.CorrelationId.ShouldBe(original.CorrelationId);
        }

        [Test]
        public void should_be_sent_back_to_the_requester()
        {
            theSentEnvelope.Destination.ShouldBe(original.ReplyUri);
        }

        [Test]
        public void the_response_id_going_back_should_be_the_original_correlation_id()
        {
            theSentEnvelope.ResponseId.ShouldBe(original.CorrelationId);
        }
    }


    [TestFixture]
    public class when_sending_a_failure_ack_and_response_is_requested
    {
        private FailureAcknowledgement theAck;
        private Envelope theSentEnvelope;
        private Envelope original;

        [SetUp]
        public void SetUp()
        {
            original = new Envelope
            {
                ReplyUri = "foo://bar".ToUri(),
                AckRequested = false,
                ReplyRequested = "Message1",
                CorrelationId = Guid.NewGuid().ToString()
            };

            var recordingSender = new RecordingEnvelopeSender();
            new OutgoingSender(recordingSender, new RecordingLogger())
                .SendFailureAcknowledgement(original, "you stink");

            theSentEnvelope = recordingSender.Sent.Single();
            theAck = theSentEnvelope.Message as FailureAcknowledgement;
        }

        [Test]
        public void should_have_sent_a_failure_ack()
        {
            theAck.ShouldNotBeNull();
        }

        [Test]
        public void the_message_should_be_what_was_requested()
        {
            theAck.Message.ShouldBe("you stink");
        }

        [Test]
        public void should_have_The_correlation_id_from_the_original_envelope()
        {
            theAck.CorrelationId.ShouldBe(original.CorrelationId);
        }

        [Test]
        public void should_be_sent_back_to_the_requester()
        {
            theSentEnvelope.Destination.ShouldBe(original.ReplyUri);
        }

        [Test]
        public void the_response_id_going_back_should_be_the_original_correlation_id()
        {
            theSentEnvelope.ResponseId.ShouldBe(original.CorrelationId);
        }
    }

}