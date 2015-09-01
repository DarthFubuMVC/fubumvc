using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
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

        [Test]
        public void should_have_delegated_to_all_inners()
        {
            inners.Each(inner => {
                inner.AssertWasCalled(x => x.Execute(envelope, theContext));
            });
        }
    }
}