using FubuCore;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Async
{
    [TestFixture]
    public class AsyncChainExecutionContinuationTester
    {
        [Test]
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