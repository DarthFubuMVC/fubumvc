using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    
    public class PartialChainSwitcherTester : InteractionContext<PartialChainSwitcher>
    {
        private BehaviorChain theTarget;
        private ICurrentChain theChain;
        private IActionBehavior theInner;

        protected override void beforeEach()
        {
            theTarget = BehaviorChain.For<ControllerTarget>(x => x.ZeroInOneOut());
            theChain = MockFor<ICurrentChain>();
            theInner = MockFor<IActionBehavior>();

            Container.Inject(theTarget);
            ClassUnderTest.InvokePartial();
        }

        [Fact]
        public void invoke_partial_causes_chain_context_switch()
        {
            theChain.AssertWasCalled(x => x.Push(theTarget));
            theInner.AssertWasCalled(x => x.InvokePartial());
            theChain.AssertWasCalled(x => x.Pop());
        }

        [Fact]
        public void invoke_throws_an_exception()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.Invoke();
            });
        }
    }
}