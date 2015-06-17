using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Testing.ScenarioSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuTransportation.Testing.Runtime.Invocation
{
    [TestFixture]
    public class WhenInvokingWithNoHandlerForMessageType : InteractionContext<HandlerPipeline>
    {
        private HandlerGraph theGraph;

        protected override void beforeEach()
        {
            theGraph = FubuTransportRegistry.HandlerGraphFor(x =>
            {
                x.Handlers.DisableDefaultHandlerSource();
                x.Handlers.Include<OneHandler>();
                x.Handlers.Include<TwoHandler>();
                x.Handlers.Include<ThreeHandler>();
                x.Handlers.Include<FourHandler>();
            });

            Services.Inject<HandlerGraph>(theGraph);
        }


        [Test]
        public void should_move_message_to_error_queue()
        {
            var envelope = ObjectMother.Envelope();
            envelope.Message = new Message1();
            ClassUnderTest.Invoke(envelope); // we don't have a handler for this type

            envelope.Callback.AssertWasCalled(x => x.MoveToErrors(
                new ErrorReport(envelope, new NoHandlerException(typeof(Message1)))));
        }
    }
}