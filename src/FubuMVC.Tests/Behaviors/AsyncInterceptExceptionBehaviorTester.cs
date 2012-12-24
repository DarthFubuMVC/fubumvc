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
    public class when_an_exception_is_thrown_and_should_not_be_handled : InteractionContext<AsyncInterceptExceptionBehavior>
    {
        private IExceptionHandler _exceptionHandler;
        private IExceptionHandlingObserver _observer;
        private AggregateException _exception;
        private ArgumentException _argumentException;

        protected override void beforeEach()
        {
            _argumentException = new ArgumentException("Failed");
            _exceptionHandler = MockFor<IExceptionHandler>();
            _observer = MockFor<IExceptionHandlingObserver>();
            _exceptionHandler.Expect(x => x.ShouldHandle(_argumentException)).Return(false);
            ClassUnderTest.InsideBehavior = new AsyncThrowingBehavior(_argumentException);
            var testTask = new Task(() => ClassUnderTest.Invoke());
            testTask.Start();
            _exception = Assert.Throws<AggregateException>(testTask.Wait);
        }

        [Test]
        public void should_not_call_handle()
        {
            _exceptionHandler.VerifyAllExpectations();
        }

        [Test]
        public void should_not_be_observed_as_handled()
        {
            _observer.VerifyAllExpectations();
        }

        [Test]
        public void should_have_the_correct_exception()
        {
            _exception.InnerExceptions.Contains(_argumentException);
        }
    }

    [TestFixture]
    public class when_an_exception_is_thrown_and_should_be_handled : InteractionContext<AsyncInterceptExceptionBehavior>
    {
        private IExceptionHandler _exceptionHandler;
        private IExceptionHandlingObserver _observer;
        private AggregateException _exception;
        private ArgumentException _argumentException;

        protected override void beforeEach()
        {
            _argumentException = new ArgumentException("Failed");
            _exceptionHandler = MockFor<IExceptionHandler>();
            _observer = MockFor<IExceptionHandlingObserver>();
            _observer.Expect(x => x.RecordHandled(_argumentException));
            _exceptionHandler.Expect(x => x.ShouldHandle(_argumentException)).Return(true);
            _exceptionHandler.Expect(x => x.Handle(_argumentException));
            ClassUnderTest.InsideBehavior = new AsyncThrowingBehavior(_argumentException);
            var testTask = new Task(() => ClassUnderTest.Invoke());
            testTask.Start();
            _exception = Assert.Throws<AggregateException>(testTask.Wait);
        }

        [Test]
        public void should_call_handle()
        {
            _exceptionHandler.VerifyAllExpectations();
        }

        [Test]
        public void should_be_observed_as_handled()
        {
            _observer.VerifyAllExpectations();
        }

        [Test]
        public void should_have_the_correct_exception()
        {
            _exception.InnerExceptions.Contains(_argumentException);
        }
    }

    [TestFixture]
    public class when_no_exception_is_thrown : InteractionContext<AsyncInterceptExceptionBehavior>
    {
        private IExceptionHandler _exceptionHandler;
        private IExceptionHandlingObserver _observer;
        private DoNothingBehavior _insideBehavior;

        protected override void beforeEach()
        {
            _insideBehavior = new DoNothingBehavior();
            _exceptionHandler = MockFor<IExceptionHandler>();
            _observer = MockFor<IExceptionHandlingObserver>();
            ClassUnderTest.InsideBehavior = _insideBehavior;
            var testTask = new Task(() => ClassUnderTest.Invoke());
			testTask.RunSynchronously();
        }

        [Test]
        public void should_not_call_handle()
        {
            _exceptionHandler.VerifyAllExpectations();
        }

        [Test]
        public void should_not_be_observed_as_handled()
        {
            _observer.VerifyAllExpectations();
        }

        [Test]
        public void inside_behavior_should_be_invoked()
        {
            _insideBehavior.Invoked.ShouldBeTrue();
        }
    }

    public class AsyncThrowingBehavior : IActionBehavior
    {
        private readonly Exception _exception;

        public AsyncThrowingBehavior(Exception exception)
        {
            _exception = exception;
        }

        public void Invoke()
        {
            Task.Factory.StartNew(() => { throw _exception; }, TaskCreationOptions.AttachedToParent);
        }

        public void InvokePartial()
        {
            Task.Factory.StartNew(() => { throw _exception; }, TaskCreationOptions.AttachedToParent);
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
