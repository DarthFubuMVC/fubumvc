using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Runtime.Conditionals;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class when_condition_is_false : InteractionContext<ConditionalBehavior>
    {
        protected override void beforeEach()
        {
            MockFor<IConditional>().Stub(x => x.ShouldExecute()).Return(false);
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