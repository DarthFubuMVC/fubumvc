using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class when_tracing_through_a_successful_behavior : InteractionContext<BehaviorTracer>
    {
        private IActionBehavior inner;

        protected override void beforeEach()
        {
            inner = MockFor<IActionBehavior>();
            ClassUnderTest.Inner = inner;
            
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

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
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
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
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(true);
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

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
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.MarkException(exception));
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
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
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

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

        protected override void beforeEach()
        {
            exception = new NotImplementedException();
            inner = MockFor<IActionBehavior>();
            inner.Expect(x => x.InvokePartial()).Throw(exception);
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(true);
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

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
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.MarkException(exception));
        }

        [Test]
        public void should_mark_the_inner_behavior_as_complete_with_the_debug_report()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
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
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

            ClassUnderTest.Inner = inner;
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
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().Stub(x => x.StartBehavior(inner)).Return(new BehaviorReport(inner));

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
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.EndBehavior());
        }

        [Test]
        public void should_register_a_new_behavior_running()
        {
            Assert.Fail("NWO");
            //MockFor<IDebugReport>().AssertWasCalled(x => x.StartBehavior(inner));
        }
    }
}
