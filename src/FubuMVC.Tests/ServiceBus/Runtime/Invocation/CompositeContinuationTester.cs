using System;
using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class CompositeContinuationTester : InteractionContext<CompositeContinuation>
    {
        private IContinuation[] inners;
        private Envelope envelope;
        private TestEnvelopeContext theContext;

        protected override void beforeEach()
        {
            inners = Services.CreateMockArrayFor<IContinuation>(5);

            envelope = new Envelope();
            theContext = new TestEnvelopeContext();

            ClassUnderTest.Execute(envelope, theContext);
        }

        [Fact]
        public void should_have_delegated_to_all_inners()
        {
            inners.Each(inner => {
                inner.AssertWasCalled(x => x.Execute(envelope, theContext));
            });
        }
    }


}