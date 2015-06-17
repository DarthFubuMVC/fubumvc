using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class when_invoking_a_message_right_now_happy_path : InteractionContext<ChainInvoker>
    {
        private OneMessage theMessage;
        private HandlerGraph theGraph;
        private HandlerChain theExpectedChain;
        private StubServiceFactory theFactory;
        private object[] cascadingMessages;

        protected override void beforeEach()
        {
            theMessage = new OneMessage();

            ClassUnderTest.InvokeNow(theMessage);
        }

        [Test]
        public void invoked_through_the_pipeline()
        {
            MockFor<IHandlerPipeline>().AssertWasCalled(x => 
                x.Invoke(Arg<Envelope>.Matches(e => e.Message == theMessage)));
        }
    }
}