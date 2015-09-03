using System;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class ChainSuccessContinuationTester
    {
        private Envelope theEnvelope;
        private FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext theContext;
        private RecordingEnvelopeSender theSender;
        private ChainSuccessContinuation theContinuation;
        private RecordingLogger theLogger;
        private TestEnvelopeContext theEnvelopeContext;

        [SetUp]
        public void SetUp()
        {
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Message = new object();

            theEnvelopeContext = new TestEnvelopeContext();

            theContext = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(theEnvelope, new HandlerChain());

            theContext.EnqueueCascading(new object());
            theContext.EnqueueCascading(new object());
            theContext.EnqueueCascading(new object());

            theSender = new RecordingEnvelopeSender();

            theContinuation = new ChainSuccessContinuation(theContext);

            theLogger = new RecordingLogger();

            theContinuation.Execute(theEnvelope, theEnvelopeContext);
        }

        [Test]
        public void should_mark_the_message_as_successful()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MarkSuccessful());
        }

        [Test]
        public void should_log_the_chain_success()
        {
            theEnvelopeContext.RecordedLogs.InfoMessages.Single()
                .ShouldBe(new MessageSuccessful { Envelope = theEnvelope.ToToken() });
        }
    }

    [TestFixture]
    public class ChainSuccessContinuation_failure_Tester
    {
        private Envelope theEnvelope;
        private FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext theContext;
        private ChainSuccessContinuation theContinuation;
        private TestEnvelopeContext theEnvelopeContext;
        private Exception theException;

        [SetUp]
        public void SetUp()
        {
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Message = new object();
            theException = new Exception("Failure");
            theEnvelope.Callback.Stub(x => x.MarkSuccessful()).Throw(theException);

            theEnvelopeContext = new TestEnvelopeContext();

            theContext = new FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext(theEnvelope, new HandlerChain());
            theContext.EnqueueCascading(new object());

            theContinuation = new ChainSuccessContinuation(theContext);
            theContinuation.Execute(theEnvelope, theEnvelopeContext);
        }

        [Test]
        public void should_not_log_success()
        {
            theEnvelopeContext.RecordedLogs.InfoMessages.ShouldNotContain(
                new MessageSuccessful { Envelope = theEnvelope.ToToken() });
        }

        [Test]
        public void should_move_the_envelope_to_the_error_queue()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MoveToErrors(new ErrorReport(theEnvelope, theException)));
        }

        [Test]
        public void should_log_the_exception()
        {
            var report = theEnvelopeContext.RecordedLogs.ErrorMessages.Single()
                .As<ExceptionReport>();

            report.ExceptionText.ShouldBe(theException.ToString());
        }

        [Test]
        public void should_send_a_failure_ack()
        {
            theEnvelopeContext.RecordedOutgoing.FailureAcknowledgementMessage
                .ShouldBe("Sending cascading message failed: Failure");
        }
    }
}