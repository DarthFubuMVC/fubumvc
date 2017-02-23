using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    
    public class CascadingHandlerInvokerTester : InteractionContext<CascadingHandlerInvoker<ITargetHandler,Input,Output>>
    {
        private Input theInput;
        private Output expectedOutput;

        protected override void beforeEach()
        {
            Func<ITargetHandler, Input, Output> func = (c, i) => c.OneInOneOut(i);

            Services.Inject<Func<ITargetHandler, Input, Output>>(func);

            theInput = new Input();
            expectedOutput = new Output();

            var request = new InMemoryFubuRequest();
            request.Set(theInput);
            Services.Inject<IFubuRequest>(request);

            MockFor<ITargetHandler>().Expect(x => x.OneInOneOut(theInput)).Return(expectedOutput);

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_invoke_the_handler_method()
        {
            VerifyCallsFor<ITargetHandler>();
        }

        [Fact]
        public void records_the_output_back_to_outgoing_messages()
        {
            MockFor<IInvocationContext>().AssertWasCalled(x => x.EnqueueCascading(expectedOutput));
        }

        [Fact]
        public void continues_on_to_the_inside_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

    
    public class CascadingHandlerInvoker_with_base_class_Tester : InteractionContext<CascadingHandlerInvoker<ITargetHandler, Input, Output>>
    {
        private SpecialInput theInput;
        private Output expectedOutput;

        protected override void beforeEach()
        {
            Func<ITargetHandler, Input, Output> func = (c, i) => c.OneInOneOut(i);

            Services.Inject<Func<ITargetHandler, Input, Output>>(func);

            theInput = new SpecialInput();
            expectedOutput = new Output();

            var request = new InMemoryFubuRequest();
            request.Set(theInput);
            Services.Inject<IFubuRequest>(request);

            MockFor<ITargetHandler>().Expect(x => x.OneInOneOut(theInput)).Return(expectedOutput);

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_invoke_the_controller_method()
        {
            VerifyCallsFor<ITargetHandler>();
        }

        [Fact]
        public void records_the_output_back_to_outgoing_messages()
        {
            MockFor<IInvocationContext>().AssertWasCalled(x => x.EnqueueCascading(expectedOutput));
        }

        [Fact]
        public void continues_on_to_the_inside_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

}