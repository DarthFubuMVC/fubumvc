using System;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.Tests;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class when_tracing_through_a_successful_behavior : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private RecordingLogger logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation();
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
            logs.DebugMessages.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.DebugMessages.First().ShouldEqual(new BehaviorStart(correlation));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_successful_behavior_as_a_partial : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private RecordingLogger logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation();
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
            logs.DebugMessages.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.DebugMessages.First().ShouldEqual(new BehaviorStart(correlation));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_that_throws_an_exception_during_a_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;
        private RecordingLogger logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation();
            Services.Inject(correlation);

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(true);

            ClassUnderTest.Inner = inner;
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            inner.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void should_mark_the_debug_report_with_the_exception()
        {
            logs.ErrorMessages.OfType<ExceptionReport>().Single()
                .ExceptionText.ShouldEqual(exception.ToString());
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            logs.DebugMessages.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.DebugMessages.First().ShouldEqual(new BehaviorStart(correlation));
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
            correlation = new BehaviorCorrelation();
            Services.Inject(correlation);

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);

            ClassUnderTest.Inner = inner;
        }

        [Test]
        public void should_allow_the_exception_to_bubble_up()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => ClassUnderTest.Invoke());
        }
    }


    [TestFixture]
    public class when_tracing_through_a_behavior_in_partial_invoke_that_throws_an_exception_during_a_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;
        private RecordingLogger logs;
        private BehaviorCorrelation correlation;

        protected override void beforeEach()
        {
            logs = Services.RecordLogging();
            correlation = new BehaviorCorrelation();
            Services.Inject(correlation);

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(true);

            ClassUnderTest.Inner = inner;
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void should_invoke_the_inner_behavior()
        {
            inner.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_mark_the_debug_report_with_the_exception()
        {
            logs.ErrorMessages.OfType<ExceptionReport>().Single()
                .ExceptionText.ShouldEqual(exception.ToString());
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            logs.DebugMessages.Last().ShouldEqual(new BehaviorFinish(correlation));
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            logs.DebugMessages.First().ShouldEqual(new BehaviorStart(correlation));
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
            correlation = new BehaviorCorrelation();
            Services.Inject(correlation);

            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);

            ClassUnderTest.Inner = inner;
        }

        [Test]
        public void should_allow_the_exception_to_bubble_up()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => ClassUnderTest.InvokePartial());
        }
    }


}
