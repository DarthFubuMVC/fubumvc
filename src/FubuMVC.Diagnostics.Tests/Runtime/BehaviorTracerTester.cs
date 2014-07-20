using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class when_tracing_through_a_successful_behavior : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private RecordingRequestTrace logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = new RecordingRequestTrace();
            Services.Inject<IRequestTrace>(logs);

            correlation = new BehaviorCorrelation(new FakeNode());
            Services.Inject(correlation);

            inner = MockFor<IActionBehavior>();
            ClassUnderTest.Inner = inner;

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            inner.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            logs.Logs.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.Logs.First().ShouldEqual(new BehaviorStart(correlation));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_successful_behavior_as_a_partial : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private RecordingRequestTrace logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = new RecordingRequestTrace();
            Services.Inject<IRequestTrace>(logs);

            correlation = new BehaviorCorrelation(new FakeNode());
            Services.Inject(correlation);

            inner = MockFor<IActionBehavior>();
            ClassUnderTest.Inner = inner;

            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            inner.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            logs.Logs.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.Logs.First().ShouldEqual(new BehaviorStart(correlation));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_that_throws_an_exception_during_a_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;
        private RecordingRequestTrace logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = new RecordingRequestTrace();
            Services.Inject<IRequestTrace>(logs);

            correlation = new BehaviorCorrelation(new FakeNode());
            Services.Inject(correlation);
            Services.Inject<IExceptionHandlingObserver>(new ExceptionHandlingObserver());

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);

            ClassUnderTest.Inner = inner;

            Exception<NotImplementedException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.Invoke();
            });
            
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            inner.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void should_mark_the_debug_report_with_the_exception()
        {
            logs.Logs.OfType<BehaviorFinish>().Single()
                .Exception.ExceptionType.ShouldEqual(exception.GetType().Name);
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            logs.Logs.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.Logs.First().ShouldEqual(new BehaviorStart(correlation));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_that_throws_an_exception_during_a_non_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;
        private BehaviorCorrelation correlation;
        private RecordingLogger logs;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation(new FakeNode());
            Services.Inject(correlation);
            Services.Inject<IExceptionHandlingObserver>(new ExceptionHandlingObserver());

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);

            ClassUnderTest.Inner = inner;
        }

        [Test]
        public void should_allow_the_exception_to_bubble_up_wrapped_in_unhandled_exception()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => ClassUnderTest.Invoke())
                .ShouldBeTheSameAs(exception);
        }
    }

    public class RecordingRequestTrace : IRequestTrace
    {
        public readonly IList<object> Logs = new List<object>();

        public RequestLog Current { get; set; }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void MarkFinished()
        {
            throw new NotImplementedException();
        }

        public void Log(object message)
        {
            Logs.Add(message);
        }

        public void MarkAsFailedRequest()
        {
            throw new NotImplementedException();
        }
    }


    [TestFixture]
    public class when_tracing_through_a_behavior_in_partial_invoke_that_throws_an_exception_during_a_non_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;
        private RecordingLogger logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation(new FakeNode());
            Services.Inject(correlation);
            Services.Inject<IExceptionHandlingObserver>(new ExceptionHandlingObserver());

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);

            ClassUnderTest.Inner = inner;
        }

        [Test]
        public void should_allow_the_exception_to_bubble_up_wrapped_in_unhandled_exception()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => ClassUnderTest.InvokePartial())
                .ShouldBeTheSameAs(exception);
        }
    }


}
