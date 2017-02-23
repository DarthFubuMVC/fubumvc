using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class DeserializationFailureContinuationTester
    {
        private EnvelopeDeserializationException theException;
        private TestEnvelopeContext theContext;
        private Envelope theEnvelope;

        public DeserializationFailureContinuationTester()
        {
            theException = new EnvelopeDeserializationException("foo");
            theContext = new TestEnvelopeContext();

            theEnvelope = ObjectMother.EnvelopeWithSerializationError();


            new DeserializationFailureContinuation(theException)
                .Execute(theEnvelope, theContext);
        }

        [Fact]
        public void should_move_the_envelope_to_the_error_queue()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MoveToErrors(new ErrorReport(theEnvelope, theException)));
        }

        [Fact]
        public void should_log_the_exception()
        {
            var report = theContext.RecordedLogs.ErrorMessages.Single()
                .As<ExceptionReport>();

            report.ExceptionText.ShouldBe(theException.ToString());
        }

        [Fact]
        public void should_send_a_failure_ack()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage.ShouldBe("Deserialization failed");

        }
    }
}