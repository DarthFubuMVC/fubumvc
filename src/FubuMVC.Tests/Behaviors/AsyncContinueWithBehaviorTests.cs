using System;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Runtime;
using FubuMVC.Tests.TestSupport;
using Shouldly;
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
            var waitHandle = new ManualResetEvent(false);
            var completion = new RequestCompletion();
            completion.WhenCompleteDo(x => waitHandle.Set());
            Services.Container.Configure(x =>
            {
                x.For<IRequestCompletion>().Use(completion);
                x.For<IAsyncCoordinator>().Use<AsyncCoordinator>();
            });
            ClassUnderTest.Inner = MockFor<IActionBehavior>();
            expectedOutput = new Output();
            var task = new Task<Output>(() => expectedOutput);
            task.RunSynchronously();

            MockFor<IFubuRequest>().Expect(x => x.Get<Task<Output>>()).Return(task);

            ClassUnderTest.Invoke();
            waitHandle.WaitOne(TimeSpan.FromSeconds(1)).ShouldBeTrue();
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
            var waitHandle = new ManualResetEvent(false);
            var completion = new RequestCompletion();
            completion.WhenCompleteDo(x => waitHandle.Set());
            Services.Container.Configure(x =>
            {
                x.For<IRequestCompletion>().Use(completion);
                x.For<IAsyncCoordinator>().Use<AsyncCoordinator>();
            });
            expectedOutput = new Output();
            var task = Task<Output>.Factory.StartNew(() =>
            {
                throw new Exception("Failed!");
            });

            MockFor<IFubuRequest>().Expect(x => x.Get<Task<Output>>()).Return(task);
            ClassUnderTest.Invoke();
            waitHandle.WaitOne(TimeSpan.FromSeconds(1)).ShouldBeTrue();
        }

        [Test]
        public void should_not_have_stored_the_task_result_in_the_fubu_request()
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
            var waitHandle = new ManualResetEvent(false);
            var completion = new RequestCompletion();
            completion.WhenCompleteDo(x => waitHandle.Set());
            Services.Container.Configure(x =>
            {
                x.For<IRequestCompletion>().Use(completion);
                x.For<IAsyncCoordinator>().Use<AsyncCoordinator>();
            });
            var task = Task.Factory.StartNew(() =>
            {
                throw new Exception("Failed!");
            });

            MockFor<IFubuRequest>().Expect(x => x.Get<Task>()).Return(task);

            ClassUnderTest.Invoke();
            waitHandle.WaitOne(TimeSpan.FromSeconds(1)).ShouldBeTrue();
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
            var waitHandle = new ManualResetEvent(false);
            var completion = new RequestCompletion();
            completion.WhenCompleteDo(x => waitHandle.Set());
            Services.Container.Configure(x =>
            {
                x.For<IRequestCompletion>().Use(completion);
                x.For<IAsyncCoordinator>().Use<AsyncCoordinator>();
            });
            ClassUnderTest.Inner = MockFor<IActionBehavior>();

            var task = new Task(() => { });
            task.RunSynchronously();

            MockFor<IFubuRequest>().Expect(x => x.Get<Task>()).Return(task);

            ClassUnderTest.Invoke();
            waitHandle.WaitOne(TimeSpan.FromSeconds(1)).ShouldBeTrue();
        }

        [Test]
        public void should_call_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }
}