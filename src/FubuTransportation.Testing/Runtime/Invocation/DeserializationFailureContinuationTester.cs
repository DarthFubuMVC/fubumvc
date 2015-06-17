using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuTestingSupport;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Runtime.Serializers;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class DeserializationFailureContinuationTester
    {
        private EnvelopeDeserializationException theException;
        private TestContinuationContext theContext;
        private Envelope theEnvelope;

        [SetUp]
        public void SetUp()
        {
            theException = new EnvelopeDeserializationException("foo");
            theContext = new TestContinuationContext();

            theEnvelope = ObjectMother.EnvelopeWithSerializationError();


            new DeserializationFailureContinuation(theException)
                .Execute(theEnvelope, theContext);
        }

        [Test]
        public void should_move_the_envelope_to_the_error_queue()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MoveToErrors(new ErrorReport(theEnvelope, theException)));
        }

        [Test]
        public void should_log_the_exception()
        {
            var report = theContext.RecordedLogs.ErrorMessages.Single()
                .As<ExceptionReport>();

            report.ExceptionText.ShouldEqual(theException.ToString());
        }

        [Test]
        public void should_send_a_failure_ack()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage.ShouldEqual("Deserialization failed");

        }
    }
}