using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.ErrorHandling
{
    [TestFixture]
    public class RetryNowContinuationTester
    {
        [Test]
        public void just_calls_through_to_the_context_pipeline_to_do_it_again()
        {
            var continuation = new RetryNowContinuation();

            var context = new TestContinuationContext
            {
                Pipeline = MockRepository.GenerateMock<IHandlerPipeline>()
            };

            var theEnvelope = ObjectMother.Envelope();
            continuation.Execute(theEnvelope, context);

            context.Pipeline.AssertWasCalled(x => x.Invoke(theEnvelope));
        }
    }
}