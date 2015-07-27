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
    public class when_invoking_with_cascading_messages : InteractionContext<CascadingAsyncHandlerInvoker<when_invoking_with_cascading_messages.TaskAction, Message1, Message2>>
    {
        private Task<Message2> theTask;
        private Message1 theMessage;

        protected override void beforeEach()
        {
            theMessage = new Message1();
            MockFor<IFubuRequest>().Stub(x => x.Find<Message1>()).Return(new Message1[]{theMessage});


            theTask = new Task<Message2>(() => { return null; });
            MockFor<TaskAction>().Expect(x => x.Go(theMessage))
                .Return(theTask);

            Func<TaskAction, Message1, Task<Message2>> func = (a, m) => a.Go(m);
            Services.Inject(func);


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
            MockFor<TaskAction>().VerifyAllExpectations();
        }

        [Test]
        public void should_register_the_returned_task_with_async_handling_for_tracking()
        {
            MockFor<IAsyncHandling>().AssertWasCalled(x => x.Push(theTask));
        }


        public interface TaskAction
        {
            Task<Message2> Go(Message1 message);
        }
    }
}