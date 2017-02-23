using FubuMVC.Core.ServiceBus.ErrorHandling;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class RequeueContinuationTester
    {
        [Fact]
        public void executing_just_puts_it_back_in_line_at_the_back_of_the_queue()
        {
            var envelope = ObjectMother.Envelope();

            new RequeueContinuation().Execute(envelope, new TestEnvelopeContext());

            envelope.Callback.AssertWasCalled(x => x.Requeue());
        }
    }
}