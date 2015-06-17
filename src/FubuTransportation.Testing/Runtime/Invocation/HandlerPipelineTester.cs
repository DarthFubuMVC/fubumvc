using System;
using System.Collections.Generic;
using FubuCore.Logging;
using FubuTestingSupport;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Logging;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Runtime.Serializers;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class when_invoking_an_envelope : InteractionContext<HandlerPipeline>
    {
        private IContinuation theContinuation;
        private Envelope theEnvelope;
        private TestContinuationContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            theContext = new TestContinuationContext();
            Services.Inject<ContinuationContext>(theContext);

            theContinuation = MockFor<IContinuation>();
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.FindContinuation(theEnvelope))
                          .Return(theContinuation);

            ClassUnderTest.Invoke(theEnvelope );
        }

        [Test]
        public void should_have_incremented_the_attempt_count()
        {
            theEnvelope.Attempts.ShouldEqual(2);
        }

        [Test]
        public void should_set_itself_on_the_continuation_context_for_later()
        {
            theContext.Pipeline.ShouldBeTheSameAs(ClassUnderTest);
        }

        [Test]
        public void should_invoke_the_continuation()
        {
            theContinuation.AssertWasCalled(x => x.Execute(theEnvelope, theContext));
        }
    }

    [TestFixture]
    public class when_invoking_an_envelope_with_serialization_error : InteractionContext<HandlerPipeline>
    {
        private Envelope theEnvelope;
        private TestContinuationContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            theContext = new TestContinuationContext();
            Services.Inject<ContinuationContext>(theContext);

            theEnvelope = ObjectMother.EnvelopeWithSerializationError();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();
        }

        [Test]
        public void exception_is_handled()
        {
            ClassUnderTest.Invoke(theEnvelope);
            // Just testing that no exception bubbles up.
        }
    }

    [TestFixture]
    public class when_receiving_an_envelope : InteractionContext<HandlerPipeline>
    {
        private IContinuation theContinuation;
        private Envelope theEnvelope;
        private TestContinuationContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            theContext = new TestContinuationContext();
            Services.Inject<ContinuationContext>(theContext);

            theContinuation = MockFor<IContinuation>();
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.FindContinuation(theEnvelope))
                          .Return(theContinuation);

            ClassUnderTest.Receive(theEnvelope);
        }

        [Test]
        public void the_serializer_should_be_set_on_the_envelope()
        {
            var theExpectedMessage = new object();

            MockFor<IEnvelopeSerializer>().Stub(x => x.Deserialize(theEnvelope))
                                          .Return(theExpectedMessage);

            theEnvelope.Message.ShouldBeTheSameAs(theExpectedMessage);
        }

        [Test]
        public void log_the_envelope_received()
        {
            theContext.RecordedLogs.InfoMessages.ShouldContain(new EnvelopeReceived
            {
                Envelope = theEnvelope.ToToken()
            });
        }

        [Test]
        public void should_invoke()
        {
            ClassUnderTest.AssertWasCalled(x => x.Invoke(theEnvelope));
        }
    }


    [TestFixture]
    public class when_determining_the_continuation : InteractionContext<HandlerPipeline>
    {
        private IEnvelopeHandler[] theHandlers;
        private RecordingLogger theLogger;
        private IContinuation theContinuation;
        private Envelope theEnvelope;
        private IContinuation theFoundContinuation;

        protected override void beforeEach()
        {
            theHandlers = Services.CreateMockArrayFor<IEnvelopeHandler>(5);

            theLogger = new RecordingLogger();
            Services.Inject<ILogger>(theLogger);

            theContinuation = MockFor<IContinuation>();
            theEnvelope = ObjectMother.Envelope();

            theHandlers[3].Stub(x => x.Handle(theEnvelope))
                          .Return(theContinuation);

            theHandlers[4].Stub(x => x.Handle(theEnvelope))
                          .Return(MockRepository.GenerateMock<IContinuation>());

            theFoundContinuation = ClassUnderTest.FindContinuation(theEnvelope);
        }

        [Test]
        public void should_find_the_first_non_null_continuation_from_a_handler()
        {
            theFoundContinuation.ShouldBeTheSameAs(theContinuation);
        }

        [Test]
        public void should_debug_the_handler_and_continuation_used()
        {
            var log = theLogger.DebugMessages.Single().ShouldBeOfType<EnvelopeContinuationChosen>();
            log.Envelope.ShouldEqual(theEnvelope.ToToken());
            log.ContinuationType.ShouldEqual(theContinuation.GetType());
            log.HandlerType.ShouldEqual(theHandlers[3].GetType());
        }
    }

}