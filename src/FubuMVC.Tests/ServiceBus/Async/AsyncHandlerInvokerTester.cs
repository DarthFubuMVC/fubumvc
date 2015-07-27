using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Async
{
    [TestFixture]
    public class when_invoking : InteractionContext<AsyncHandlerInvoker<when_invoking.AsyncAction, Message1>>
    {
        private Task theTask;
        private Message1 theMessage;

        protected override void beforeEach()
        {
            theMessage = new Message1();
            MockFor<IFubuRequest>().Stub(x => x.Find<Message1>()).Return(new Message1[]{theMessage});

            theTask = new Task(() => { });
            Func<AsyncAction, Message1, Task> func = (a, m) => a.Go(m);
            Services.Inject(func);

            MockFor<AsyncAction>().Expect(x => x.Go(theMessage)).Return(theTask);

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
        
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void should_call_the_method_on_the_action_class()
        {
            MockFor<AsyncAction>().VerifyAllExpectations();
        }

        [Test]
        public void should_register_the_returned_task_with_async_handling_for_tracking()
        {
            MockFor<IAsyncHandling>().AssertWasCalled(x => x.Push(theTask));
        }


        public interface AsyncAction
        {
            Task Go(Message1 message);
        }


    }
}