using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class when_the_move_to_error_queue_continuation_executes
    {
        public when_the_move_to_error_queue_continuation_executes()
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

        [Fact]
        public void should_add_a_new_error_report()
        {
            var report = theEnvelope.Callback.GetArgumentsForCallsMadeOn(x => x.MoveToErrors(null))
                [0][0].As<ErrorReport>();

            report.ExceptionText.ShouldBe(theException.ToString());
        }

        [Fact]
        public void should_send_a_failure_acknowledgement()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage
                .ShouldBe("Moved message {0} to the Error Queue.\n{1}".ToFormat(theEnvelope.CorrelationId,
                    theException));
        }
    }
}