using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    [TestFixture]
    public class SimpleHandlerInvokerTester : InteractionContext<SimpleHandlerInvoker<ITargetHandler, Input>>
    {
        private Input theInput;

        protected override void beforeEach()
        {
            Action<ITargetHandler, Input> action = (c, i) => c.OneInZeroOut(i);

            Services.Inject(action);

            theInput = new Input();

            var request = new InMemoryFubuRequest();
            request.Set(theInput);
            Services.Inject<IFubuRequest>(request);

            MockFor<ITargetHandler>().Expect(x => x.OneInZeroOut(theInput));

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_handler_method()
        {
            VerifyCallsFor<ITargetHandler>();
        }

        [Test]
        public void continues_on_to_the_inside_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class SimpleHandlerInvoker_for_base_class_Tester : InteractionContext<SimpleHandlerInvoker<ITargetHandler, Input>>
    {
        private SpecialInput theInput;

        protected override void beforeEach()
        {
            Action<ITargetHandler, Input> action = (c, i) => c.OneInZeroOut(i);

            Services.Inject(action);

            theInput = new SpecialInput();

            var request = new InMemoryFubuRequest();
            request.Set(theInput);
            Services.Inject<IFubuRequest>(request);

            MockFor<ITargetHandler>().Expect(x => x.OneInZeroOut(theInput));

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_handler_method()
        {
            VerifyCallsFor<ITargetHandler>();
        }

        [Test]
        public void continues_on_to_the_inside_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }
}