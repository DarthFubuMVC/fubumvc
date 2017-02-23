﻿using System.Linq;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class when_handling_a_message
    {
        private TestEnvelopeContext theContext;
        private Envelope theEnvelope;

        public when_handling_a_message()
        {
            theContext = new TestEnvelopeContext();
            theEnvelope = ObjectMother.Envelope();

            new NoSubscriberHandler().Execute(theEnvelope, theContext);
        }

        [Fact]
        public void should_mark_the_dequeue_as_successful()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MarkSuccessful());
        }

        [Fact]
        public void should_have_sent_a_failure_ack()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage
                .ShouldBe("No subscriber");
        }

        [Fact]
        public void should_log_a_message_for_there_being_no_handler()
        {
            var log = theContext.RecordedLogs.InfoMessages.OfType<NoHandlerForMessage>()
                .Single();

            log.ShouldNotBeNull();
        }
    }
}