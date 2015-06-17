using FubuTestingSupport;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
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
                .ShouldEqual("No subscriber");
        }
    }
}