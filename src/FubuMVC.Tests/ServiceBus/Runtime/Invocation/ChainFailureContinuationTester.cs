using System;
using System.Linq;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class when_the_ChainFailureContinuation_Executes
    {
        private Exception theException;
        private ChainFailureContinuation theContinuation;
        private Envelope theEnvelope;
        private TestEnvelopeContext theContext;

        public when_the_ChainFailureContinuation_Executes()
        {
            theException = new Exception();

            theContinuation = new ChainFailureContinuation(theException);

            theEnvelope = ObjectMother.Envelope();

            theContext = new TestEnvelopeContext();

            theContinuation.Execute(theEnvelope, theContext);
        }

        [Fact]
        public void should_mark_the_envelope_as_failed()
        {
            // TODO -- should this be going to the error or dead letter queue instead?
        
            theEnvelope.Callback.AssertWasCalled(x => x.MarkFailed(theException));
        }

        [Fact]
        public void should_log_the_message_failed()
        {
            theContext.RecordedLogs.InfoMessages.Single().ShouldBe(new MessageFailed
            {
                Envelope = theEnvelope.ToToken(),
                Exception = theException
            });
        }

        [Fact]
        public void should_log_the_actual_exception()
        {
            var report = theContext.RecordedLogs.ErrorMessages.Single()
                .ShouldBeOfType<FubuCore.Logging.ExceptionReport>();

            report.ExceptionText.ShouldBe(theException.ToString());
        }

        [Fact]
        public void should_send_a_failure_ack()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage.ShouldBe("Chain execution failed");

        }
    }
}