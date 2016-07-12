using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class when_invoking_an_envelope : InteractionContext<HandlerPipeline>
    {
        private IContinuation theContinuation;
        private Envelope theEnvelope;
        private TestEnvelopeContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            theContext = new TestEnvelopeContext();
            Services.Inject<EnvelopeContext>(theContext);

            theContinuation = MockFor<IContinuation>();
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.FindContinuation(theEnvelope, theContext))
                          .Return(theContinuation.ToCompletionTask());

            ClassUnderTest.Invoke(theEnvelope, theContext).GetAwaiter().GetResult();
        }

        [Test]
        public void should_have_incremented_the_attempt_count()
        {
            theEnvelope.Attempts.ShouldBe(2);
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
        private TestEnvelopeContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            MockFor<IEnvelopeHandler>().Stub(x => x.Handle(null)).Return(Task.FromResult<IContinuation>(null)).IgnoreArguments();

            theContext = new TestEnvelopeContext();
            Services.Inject<EnvelopeContext>(theContext);

            theEnvelope = ObjectMother.EnvelopeWithSerializationError();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();
        }

        [Test]
        public void exception_is_handled()
        {
            ClassUnderTest.Invoke(theEnvelope, new TestEnvelopeContext()).Wait();
            // Just testing that no exception bubbles up.
        }
    }

    [TestFixture]
    public class when_receiving_an_envelope : InteractionContext<HandlerPipeline>
    {
        private IContinuation theContinuation;
        private Envelope theEnvelope;
        private TestEnvelopeContext theContext;

        protected override void beforeEach()
        {
            Services.Inject<IEnumerable<IEnvelopeHandler>>(new IEnvelopeHandler[0]);

            theContext = new TestEnvelopeContext();
            Services.Inject<EnvelopeContext>(theContext);

            theContinuation = MockFor<IContinuation>();
            theEnvelope = ObjectMother.Envelope();
            theEnvelope.Attempts = 1;

            theEnvelope.Callback = MockFor<IMessageCallback>();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.FindContinuation(theEnvelope, theContext))
                          .Return(theContinuation.ToCompletionTask());

            MockFor<IEnvelopeLifecycle>().Stub(x => x.StartNew(ClassUnderTest, theEnvelope))
                .Return(theContext);

            ClassUnderTest.Receive(theEnvelope).GetAwaiter().GetResult();
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
            ClassUnderTest.AssertWasCalled(x => x.Invoke(theEnvelope, theContext));
        }
    }



}