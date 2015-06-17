using System.Collections.Generic;
using FubuTestingSupport;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class CompositeContinuationTester : InteractionContext<CompositeContinuation>
    {
        private IContinuation[] inners;
        private Envelope envelope;
        private TestContinuationContext theContext;

        protected override void beforeEach()
        {
            inners = Services.CreateMockArrayFor<IContinuation>(5);

            envelope = new Envelope();
            theContext = new TestContinuationContext();

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