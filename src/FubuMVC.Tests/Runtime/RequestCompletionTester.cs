
using System;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class RequestCompletionTester
    {
        private RequestCompletion _requestCompletion;
        private Exception _exception;
        private int _completedCount;

        [SetUp]
        public void Setup()
        {
            _completedCount = 0;
            _exception = null;
            _requestCompletion = new RequestCompletion();
            _requestCompletion.WhenCompleteDo(x =>
            {
                _exception = x;
                _completedCount++;
            });
        }

        [Test]
        public void start_completes_when_synchronous()
        {
            _requestCompletion.Start(() => { });
            _completedCount.ShouldEqual(1);
        }

        [Test]
        public void no_errors_are_reported()
        {
            _requestCompletion.Start(() => { });
            _exception.ShouldBeNull();
        }

        [Test]
        public void when_an_error_occurs_it_is_not_handled()
        {
            typeof(InvalidOperationException).ShouldBeThrownBy(() => 
                _requestCompletion.Start(() => { throw new InvalidOperationException(); }));
            _exception.ShouldBeNull();
        }

        [Test]
        public void multiple_completion_subscribers_are_notified()
        {
            var completed = false;
            _requestCompletion.WhenCompleteDo(x =>
            {
                completed = true;
            });
            _requestCompletion.Start(() => { });
            completed.ShouldBeTrue();
            _completedCount.ShouldEqual(1);
        }

        [Test]
        public void safe_start_errors_should_be_handled()
        {
            _requestCompletion.SafeStart(() => {throw new InvalidOperationException();});
            _exception.ShouldNotBeNull();
            _exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Test]
        public void asynchronous_completes_when_told()
        {
            _requestCompletion.IsAsynchronous();
            _requestCompletion.Start(() => { });
            _completedCount.ShouldEqual(0);
            _requestCompletion.Complete();
            _completedCount.ShouldEqual(1);
        }

        [Test]
        public void asynchronous_completes_with_errors()
        {
            _requestCompletion.IsAsynchronous();
            _requestCompletion.Start(() => { });
            _exception.ShouldBeNull();
            _requestCompletion.CompleteWithErrors(new AggregateException());
            _exception.ShouldNotBeNull();
            _exception.ShouldBeOfType<AggregateException>();
        }
    }
}