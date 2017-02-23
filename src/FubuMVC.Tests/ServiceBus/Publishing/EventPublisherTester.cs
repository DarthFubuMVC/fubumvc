using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Publishing
{
    
    public class publishing_a_message_successfully : InteractionContext<SendMessageBehavior<Message1>>
    {
        private Message1 theMessage;
        private InMemoryFubuRequest theRequest;

        protected override void beforeEach()
        {
            theMessage = new Message1();
            theRequest = new InMemoryFubuRequest();
            theRequest.Set(theMessage);
            Services.Inject<IFubuRequest>(theRequest);

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_send_the_message_to_the_service_bus()
        {
            MockFor<IServiceBus>().AssertWasCalled(x => x.Send(theMessage));
        }

        [Fact]
        public void should_call_through_to_the_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Fact]
        public void should_return_a_successful_ajax_continuation()
        {
            var continuation = theRequest.Get<AjaxContinuation>();
            continuation.Success.ShouldBeTrue();
        }
    }
}