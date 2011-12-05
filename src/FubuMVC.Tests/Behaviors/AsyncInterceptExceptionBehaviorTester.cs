using System;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class AsyncInterceptExceptionBehaviorTester
    {
        [Test]
        public void should_invoke_inside_behavior()
        {
            var insideBehavior = new AsyncDoNothingBehavior();
            var cut = new AsyncTestInterceptExceptionBehavior<ArgumentException>
            {
                InsideBehavior = insideBehavior
            };

            var testTask = new Task(cut.Invoke);
            testTask.RunSynchronously();

            insideBehavior.Invoked.ShouldBeTrue();
        }

        [Test]
        public void when_no_exception_is_thrown_none_should_be_handled()
        {
            var insideBehavior = new AsyncDoNothingBehavior();
            var cut = new AsyncTestInterceptExceptionBehavior<ArgumentException>
            {
                InsideBehavior = insideBehavior
            };

            var testTask = new Task(cut.Invoke);
            testTask.RunSynchronously();

            cut.ShouldHandleCalled.ShouldBeFalse();
            cut.HandledException.ShouldBeNull();
        }

        [Test]
        public void invoke_should_throw_an_exception_when_no_inside_behavior_is_set()
        {
            var interceptExceptionBehavior = new AsyncTestInterceptExceptionBehavior<ArgumentException>();

            typeof(FubuAssertionException).ShouldBeThrownBy(interceptExceptionBehavior.Invoke);
        }

        [Test]
        public void when_matching_exception_is_thrown_by_inside_behavior_it_should_be_handled()
        {
            var cut = new AsyncTestInterceptExceptionBehavior<ArgumentException>
            {
                InsideBehavior = new AsyncThrowingBehavior<ArgumentException>()
            };

            var testTask = new Task(cut.Invoke);
            testTask.RunSynchronously();

            cut.HandledException.ShouldBeOfType<ArgumentException>();
        }

        [Test]
        public void when_exception_should_not_be_handled_the_handle_method_should_not_be_invoked()
        {
            var cut = new AsyncTestInterceptExceptionBehavior<ArgumentException>
            {
                InsideBehavior = new AsyncThrowingBehavior<ArgumentException>()
            };
            cut.SetShouldHandle(false);
            var testTask = new Task(cut.Invoke);
            testTask.RunSynchronously();
            cut.ShouldHandleCalled.ShouldBeTrue();
            cut.HandledException.ShouldBeNull();
        }

        [Test]
        public void when_non_matching_exception_is_thrown_should_handled_should_not_be_invoked()
        {
            var cut = new AsyncTestInterceptExceptionBehavior<ArgumentException>
            {
                InsideBehavior = new AsyncThrowingBehavior<WebException>()
            };
            cut.SetShouldHandle(false);
            var testTask = new Task(cut.Invoke);
            testTask.RunSynchronously();
            cut.ShouldHandleCalled.ShouldBeFalse();
            cut.HandledException.ShouldBeNull();
        }
    }

    public class AsyncTestInterceptExceptionBehavior<T> : AsyncInterceptExceptionBehavior<T>
        where T : Exception
    {

        private bool shouldHandle = true;
        public T HandledException { get; private set; }
        public bool ShouldHandleCalled { get; private set; }

        public void SetShouldHandle(bool value)
        {
            shouldHandle = value;
        }

        public override bool ShouldHandle(T exception)
        {
            ShouldHandleCalled = true;
            return shouldHandle;
        }

        public override void Handle(T exception)
        {
            HandledException = exception;
        }
    }

    public class AsyncThrowingBehavior<T> : IActionBehavior
        where T : Exception, new()
    {
        public void Invoke()
        {
            Task.Factory.StartNew(() => { throw new T();}, TaskCreationOptions.AttachedToParent);
        }

        public void InvokePartial()
        {
            Task.Factory.StartNew(() => { throw new T();}, TaskCreationOptions.AttachedToParent);
        }
    }

    public class AsyncDoNothingBehavior : IActionBehavior
    {
        public bool Invoked { get; set; }

        public void Invoke()
        {
            Invoked = true;
            Task.Factory.StartNew(() => { }, TaskCreationOptions.AttachedToParent);
        }

        public void InvokePartial()
        {
            Invoked = true;
            Task.Factory.StartNew(() => { }, TaskCreationOptions.AttachedToParent);
        }
    }
}