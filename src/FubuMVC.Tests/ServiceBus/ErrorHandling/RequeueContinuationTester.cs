using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class RequeueContinuationTester
    {
        [Test]
        public void executing_just_puts_it_back_in_line_at_the_back_of_the_queue()
        {
            var envelope = ObjectMother.Envelope();

            new RequeueContinuation().Execute(envelope, new TestEnvelopeContext());

            envelope.Callback.AssertWasCalled(x => x.Requeue());
        }
    }
}