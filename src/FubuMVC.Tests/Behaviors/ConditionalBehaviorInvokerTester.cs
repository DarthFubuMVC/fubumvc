using System;
using FubuMVC.Core.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class ConditionalBehaviorInvokerTester : InteractionContext<ConditionalBehaviorInvoker>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.InnerBehavior = MockFor<IActionBehavior>();
        }

        [Test]
        public void invoke_should_invoke_inner_conditional_and_inner_behavior()
        {
            //ACT
            ClassUnderTest.Invoke();

            //ASSERT
            MockFor<IConditionalBehavior>().AssertWasCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
            MockFor<IConditionalBehavior>().AssertWasNotCalled(x => x.InvokePartial());
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
        [Test]
        public void invokepartial_should_invokepartial_for_inner_conditional_and_inner_behavior()
        {
            //ACT
            ClassUnderTest.InvokePartial();

            //ASSERT
            MockFor<IConditionalBehavior>().AssertWasCalled(x => x.InvokePartial());
            MockFor<IActionBehavior>().AssertWasCalled(x => x.InvokePartial());
            MockFor<IConditionalBehavior>().AssertWasNotCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }
    }

    public class when_condition_is_true : InteractionContext<ConditionalBehavior>
    {
        protected override void beforeEach()
        {
            Services.Inject<IConditional>(new LambdaConditional(() => true));
        }

        [Test]
        public void inner_behavior_should_call_invoke()
        {
            //ACT
            ClassUnderTest.Invoke();
            
            //ASSERT
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
        [Test]
        public void inner_behavior_should_call_invoke_partial()
        {
            //ACT
            ClassUnderTest.InvokePartial();

            //ASSERT
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasCalled(x => x.InvokePartial());
        }
    }

    public class when_condition_is_false : InteractionContext<ConditionalBehavior>
    {
        protected override void beforeEach()
        {
            Services.Inject<IConditional>(new LambdaConditional(() => false));
        }

        [Test]
        public void inner_behavior_should_not_call_invoke()
        {
            //ACT
            ClassUnderTest.Invoke();

            //ASSERT
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
        [Test]
        public void inner_behavior_should_not_call_invoke_partial()
        {
            //ACT
            ClassUnderTest.InvokePartial();

            //ASSERT
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.InvokePartial());
        }
    }
}