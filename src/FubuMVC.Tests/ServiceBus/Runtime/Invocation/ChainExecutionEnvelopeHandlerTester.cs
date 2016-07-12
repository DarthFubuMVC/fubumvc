using System;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
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
    public class when_the_chain_does_not_exist : InteractionContext<ChainExecutionEnvelopeHandler>
    {
        private Envelope theEnvelope;
        private IChainInvoker theInvoker;

        protected override void beforeEach()
        {
            theEnvelope = ObjectMother.Envelope();
            theInvoker = MockFor<IChainInvoker>();

            theInvoker.Stub(x => x.FindChain(theEnvelope))
                      .Return(null);
        }

        [Test]
        public void should_not_return_any_continuation()
        {
            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult().ShouldBeNull();
        }
    }


    [TestFixture]
    public class when_there_is_a_chain : InteractionContext<ChainExecutionEnvelopeHandler>
    {
        private Envelope theEnvelope;
        private IChainInvoker theInvoker;
        private HandlerChain theChain;

        protected override void beforeEach()
        {
            theEnvelope = ObjectMother.Envelope();
            theInvoker = MockFor<IChainInvoker>();
            theChain = new HandlerChain();

            theInvoker.Stub(x => x.FindChain(theEnvelope))
                      .Return(theChain);
        }

        [Test]
        public void if_the_chain_invocation_succeeds_and_there_is_no_explicit_continuation_use_successful_continuation()
        {
            theInvoker.Expect(x => x.ExecuteChain(theEnvelope, theChain))
                      .Return(MockFor<IInvocationContext>().ToCompletionTask());

            MockFor<IInvocationContext>().Stub(x => x.Continuation).Return(null);


            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult()
                          .ShouldBeOfType<ChainSuccessContinuation>()
                          .Context.ShouldBeTheSameAs(MockFor<IInvocationContext>());

        }

        [Test]
        public void if_the_chain_invocation_succeeds_and_there_is_an_explicit_continuation()
        {
            theInvoker.Expect(x => x.ExecuteChain(theEnvelope, theChain))
                      .Return(MockFor<IInvocationContext>().ToCompletionTask());

            var explicitContinuation = MockRepository.GenerateMock<IContinuation>();
            MockFor<IInvocationContext>().Stub(x => x.Continuation).Return(explicitContinuation);


            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult()
                          .ShouldBeTheSameAs(explicitContinuation);
        }

        [Test]
        public void if_the_chain_invocation_blows_up_return_a_chain_failure_continuation()
        {
            var exception = new NotImplementedException();

            theInvoker.Expect(x => x.ExecuteChain(theEnvelope, theChain))
                      .Throw(exception);

            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult()
                          .ShouldBeOfType<ChainFailureContinuation>()
                          .Exception.ShouldBeTheSameAs(exception);

        }

        [Test]
        public void if_the_chain_throws_a_deserialization_continuation()
        {
            var exception = new EnvelopeDeserializationException("I blew up!");

            theInvoker.Expect(x => x.ExecuteChain(theEnvelope, theChain))
                .Throw(exception);

            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult()
                .ShouldBeOfType<DeserializationFailureContinuation>()
                .Exception.ShouldBeTheSameAs(exception);
        }
    }


    [TestFixture]
    public class when_finding_a_chain_errors : InteractionContext<ChainExecutionEnvelopeHandler>
    {
        private Envelope theEnvelope;
        private IChainInvoker theInvoker;
        private EnvelopeDeserializationException theException;

        protected override void beforeEach()
        {
            theEnvelope = ObjectMother.Envelope();
            theInvoker = MockFor<IChainInvoker>();

            theException = new EnvelopeDeserializationException("I failed!");
            theInvoker.Stub(x => x.FindChain(theEnvelope))
                .Throw(theException);
        }

        [Test]
        public void returns_a_deserialization_failure_continuation()
        {
            ClassUnderTest.Handle(theEnvelope).GetAwaiter().GetResult()
                .ShouldBeOfType<DeserializationFailureContinuation>()
                .Exception.ShouldBeTheSameAs(theException);
        }
    }
}