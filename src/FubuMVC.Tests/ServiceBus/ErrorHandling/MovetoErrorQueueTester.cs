using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class when_the_move_to_error_queue_continuation_executes
    {
        [SetUp]
        public void SetUp()
        {
            theEnvelope = ObjectMother.Envelope();
            theException = new NotImplementedException();

            theLogger = MockRepository.GenerateMock<ILogger>();

            theContext = new TestEnvelopeContext();

            new MoveToErrorQueue(theException).Execute(theEnvelope, theContext);
        }

        private Envelope theEnvelope;
        private NotImplementedException theException;
        private ILogger theLogger;
        private TestEnvelopeContext theContext;

        [Test]
        public void should_add_a_new_error_report()
        {
            var report = theEnvelope.Callback.GetArgumentsForCallsMadeOn(x => x.MoveToErrors(null))
                [0][0].As<ErrorReport>();

            report.ExceptionText.ShouldBe(theException.ToString());
        }

        [Test]
        public void should_send_a_failure_acknowledgement()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage
                .ShouldBe("Moved message {0} to the Error Queue.\n{1}".ToFormat(theEnvelope.CorrelationId,
                    theException));
        }
    }
}