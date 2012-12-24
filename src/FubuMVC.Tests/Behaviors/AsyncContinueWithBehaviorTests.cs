using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class async_with_output_no_errors : InteractionContext<AsyncContinueWithBehavior<Output>>
    {
        private Output expectedOutput;

        protected override void beforeEach()
        {
            ClassUnderTest.Inner = MockFor<IActionBehavior>();

            expectedOutput = new Output();
            var testTask = new Task(() =>
            {
                var task = new Task<Output>(() => expectedOutput);
                task.RunSynchronously();

                MockFor<IFubuRequest>().Expect(x => x.Get<Task<Output>>()).Return(task);

                ClassUnderTest.Invoke();
            });
            testTask.RunSynchronously();
        }

        [Test]
        public void should_have_stored_the_task_result_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(expectedOutput));
        }

        [Test]
        public void should_call_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }


    [TestFixture]
    public class async_with_output_throws_error : InteractionContext<AsyncContinueWithBehavior<Output>>
    {
        private Output expectedOutput;

        protected override void beforeEach()
        {
            var testTask = new Task(() =>
            {
                expectedOutput = new Output();
                var task = Task<Output>.Factory.StartNew(() =>
                {
                    throw new Exception("Failed!");
                });

                MockFor<IFubuRequest>().Expect(x => x.Get<Task<Output>>()).Return(task);

                typeof (AggregateException).ShouldBeThrownBy(task.Wait);
                ClassUnderTest.Invoke();
            });
            testTask.RunSynchronously();
        }

        [Test]
        public void should_have_stored_the_task_result_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasNotCalled(x => x.Set(expectedOutput));
        }

        [Test]
        public void should_not_call_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class async_with_no_output_throws_error : InteractionContext<AsyncContinueWithBehavior>
    {
        protected override void beforeEach()
        {
            var testTask = new Task(() =>
            {
                var task = Task.Factory.StartNew(() =>
                {
                    throw new Exception("Failed!");
                });

                MockFor<IFubuRequest>().Expect(x => x.Get<Task>()).Return(task);

                typeof (AggregateException).ShouldBeThrownBy(task.Wait);
                ClassUnderTest.Invoke();
            });
            testTask.RunSynchronously();
        }

        [Test]
        public void should_not_call_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class async_with_no_output_no_errors : InteractionContext<AsyncContinueWithBehavior>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Inner = MockFor<IActionBehavior>();

            var testTask = new Task(() =>
            {
                var task = Task.Factory.StartNew(() => { });

                MockFor<IFubuRequest>().Expect(x => x.Get<Task>()).Return(task);

                task.Wait();
                ClassUnderTest.Invoke();
            });
            testTask.RunSynchronously();
        }

        [Test]
        public void should_call_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }
}