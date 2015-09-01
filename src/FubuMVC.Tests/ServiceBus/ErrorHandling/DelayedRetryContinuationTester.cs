using FubuCore;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class DelayedRetryContinuationTester
    {
        [Test]
        public void do_as_a_delay_w_the_timespan_given()
        {
            var continuation = new DelayedRetryContinuation(5.Minutes());
            var context = new TestEnvelopeContext();

            var envelope = ObjectMother.Envelope();

            continuation.Execute(envelope, context);

            envelope.Callback.AssertWasCalled(x => x.MoveToDelayedUntil(context.SystemTime.UtcNow().AddMinutes(5)));
        }
    }
}