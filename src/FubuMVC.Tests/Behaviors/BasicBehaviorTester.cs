using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    public abstract class non_partial_implementation_of_BasicBehavior : InteractionContext<TestBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            PartialBehaviorProvider provider = () => PartialBehavior.Ignored;
            Container.Configure(x => x.For(typeof(PartialBehaviorProvider)).Use(provider));

            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }
    }

    public abstract class partial_implementation_of_BasicBehavior : InteractionContext<TestBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            PartialBehaviorProvider provider = () => PartialBehavior.Executes;
            Container.Configure(x => x.For(typeof(PartialBehaviorProvider)).Use(provider));


            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }
    }

    [TestFixture]
    public class when_calling_Invoke_on_non_partial : non_partial_implementation_of_BasicBehavior
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_overriden_performInvoke_and_afterInsideBehavior()
        {
            MockFor<IDoSomethingBeforeInnerBehavior>().AssertWasCalled(x => x.Do());
            MockFor<IDoSomethingAfterInnerBehavior>().AssertWasCalled(x => x.Do());
        }

        [Test]
        public void it_should_call_Invoke_on_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class when_calling_InvokePartial_on_non_partial : non_partial_implementation_of_BasicBehavior
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_not_call_the_overriden_performInvoke_nor_afterInsideBehavior()
        {
            MockFor<IDoSomethingBeforeInnerBehavior>().AssertWasNotCalled(x => x.Do());
            MockFor<IDoSomethingAfterInnerBehavior>().AssertWasNotCalled(x => x.Do());
        }

        [Test]
        public void it_should_call_InvokePartial_on_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.InvokePartial());
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_calling_Invoke_on_partial : partial_implementation_of_BasicBehavior
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_overriden_performInvoke_and_afterInsideBehavior()
        {
            MockFor<IDoSomethingBeforeInnerBehavior>().AssertWasCalled(x => x.Do());
            MockFor<IDoSomethingAfterInnerBehavior>().AssertWasCalled(x => x.Do());
        }

        [Test]
        public void it_should_call_Invoke_on_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class when_calling_InvokePartial_on_partial : partial_implementation_of_BasicBehavior
    {
        protected override void beforeEach()
        {
            base.beforeEach();

            


            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_overriden_performInvoke_and_afterInsideBehavior()
        {
            MockFor<IDoSomethingBeforeInnerBehavior>().AssertWasCalled(x => x.Do());
            MockFor<IDoSomethingAfterInnerBehavior>().AssertWasCalled(x => x.Do());
        }

        [Test]
        public void it_should_call_InvokePartial_on_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.InvokePartial());
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
        }
    }

    public interface IDoSomethingBeforeInnerBehavior { void Do(); }
    public interface IDoSomethingAfterInnerBehavior { void Do(); }
    public delegate PartialBehavior PartialBehaviorProvider();

    public class TestBehavior : BasicBehavior
    {
        readonly IDoSomethingBeforeInnerBehavior _before;
        readonly IDoSomethingAfterInnerBehavior _after;

        public TestBehavior(IDoSomethingBeforeInnerBehavior before, IDoSomethingAfterInnerBehavior after,
            PartialBehaviorProvider partialBehaviorProvider) : base(partialBehaviorProvider())
        {
            _before = before;
            _after = after;
        }

        protected override DoNext performInvoke()
        {
            _before.Do();
            return DoNext.Continue;
        }

        protected override void afterInsideBehavior()
        {
            _after.Do();
        }
    }
}