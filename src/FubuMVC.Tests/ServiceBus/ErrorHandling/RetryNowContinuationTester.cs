﻿using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class RetryNowContinuationTester
    {
        [Fact]
        public void just_calls_through_to_the_context_pipeline_to_do_it_again()
        {
            var continuation = new RetryNowContinuation();

            var handlerPipeline = MockRepository.GenerateMock<IHandlerPipeline>();
            var context = new TestEnvelopeContext(handlerPipeline);

            var theEnvelope = ObjectMother.Envelope();
            continuation.Execute(theEnvelope, context);

            handlerPipeline.AssertWasCalled(x => x.Invoke(theEnvelope, context));
        }
    }
}