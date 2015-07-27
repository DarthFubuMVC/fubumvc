using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class when_handling_a_message
    {
        private TestContinuationContext theContext;
        private Envelope theEnvelope;

        [SetUp]
        public void SetUp()
        {
            theContext = new TestContinuationContext();
            theEnvelope = ObjectMother.Envelope();

            new NoSubscriberHandler().Execute(theEnvelope, theContext);
        }

        [Test]
        public void should_mark_the_dequeue_as_successful()
        {
            theEnvelope.Callback.AssertWasCalled(x => x.MarkSuccessful());
        }

        [Test]
        public void should_have_sent_a_failure_ack()
        {
            theContext.RecordedOutgoing.FailureAcknowledgementMessage
                .ShouldBe("No subscriber");
        }
    }
}