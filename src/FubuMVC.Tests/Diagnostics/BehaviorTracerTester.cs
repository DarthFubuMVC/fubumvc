using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class when_tracing_through_a_successful_behavior : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;

        protected override void beforeEach()
        {
            inner = MockFor<IActionBehavior>();
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
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_that_throws_an_exception_during_a_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;

        protected override void beforeEach()
        {
            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(true);

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
            MockFor<IDebugReport>().AssertWasCalled(x => x.MarkException(exception));
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_that_throws_an_exception_during_a_non_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;

        protected override void beforeEach()
        {
            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.Invoke()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(false);
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

        protected override void beforeEach()
        {
            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(true);

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
            MockFor<IDebugReport>().AssertWasCalled(x => x.MarkException(exception));
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
        }
    }

    [TestFixture]
    public class when_tracing_through_a_behavior_in_partial_invoke_that_throws_an_exception_during_a_non_debug_request :
        InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;
        private NotImplementedException exception;

        protected override void beforeEach()
        {
            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsDebugCall()).Return(false);
        }

        [Test]
        public void should_allow_the_exception_to_bubble_up()
        {
            Exception<NotImplementedException>.ShouldBeThrownBy(() => ClassUnderTest.InvokePartial());
        }
    }

    [TestFixture]
    public class when_tracing_through_a_successful_behavior_in_partial_invoke : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;

        protected override void beforeEach()
        {
            inner = MockFor<IActionBehavior>();
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
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
        }
    }
}