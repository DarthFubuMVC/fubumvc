using System;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

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
            _completedCount.ShouldBe(1);
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
            Exception<InvalidOperationException>.ShouldBeThrownBy(() => 
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
            _completedCount.ShouldBe(1);
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
            var trackRequestCompletion = MockRepository.GenerateMock<ITrackRequestCompletion>();
            trackRequestCompletion.Expect(x => x.IsComplete()).Return(false);
            _requestCompletion.TrackRequestCompletionWith(trackRequestCompletion);
            _requestCompletion.Start(() => { });
            _completedCount.ShouldBe(0);
            _requestCompletion.Complete();
            _completedCount.ShouldBe(1);
        }

        [Test]
        public void asynchronous_completes_with_errors()
        {
            var trackRequestCompletion = MockRepository.GenerateMock<ITrackRequestCompletion>();
            trackRequestCompletion.Expect(x => x.IsComplete()).Return(false);
            _requestCompletion.TrackRequestCompletionWith(trackRequestCompletion);
            _requestCompletion.Start(() => { });
            _exception.ShouldBeNull();
            _requestCompletion.CompleteWithErrors(new AggregateException());
            _exception.ShouldNotBeNull();
            _exception.ShouldBeOfType<AggregateException>();
        }
    }
}