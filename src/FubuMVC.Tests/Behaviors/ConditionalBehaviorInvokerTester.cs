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
}