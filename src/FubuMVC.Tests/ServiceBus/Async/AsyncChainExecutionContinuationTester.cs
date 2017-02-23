using FubuCore;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Async
{
    
    public class AsyncChainExecutionContinuationTester
    {
        [Fact]
        public void executing()
        {
            var envelope = ObjectMother.Envelope();
            var context = new TestEnvelopeContext();

            var inner = MockRepository.GenerateMock<IContinuation>();

            var continuation = new AsyncChainExecutionContinuation(() => inner);
            continuation.Execute(envelope, context);

            continuation.Task.Wait(1.Seconds());

            inner.AssertWasCalled(x => x.Execute(envelope, context));
        }
    }
}