using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class execute_one_in_one_out : InteractionContext<OneInOneOutActionInvoker<ITargetController, Input, Output>>
    {
        private Input theInput;
        private Output expectedOutput;


        protected override void beforeEach()
        {
            Func<ITargetController, Input, Output> func = (c, i) => c.OneInOneOut(i);

            Services.Inject(func);

            theInput = new Input();
            expectedOutput = new Output();

            MockFor<IFubuRequest>().Expect(x => x.Get<Input>()).Return(theInput);
            MockFor<ITargetController>().Expect(x => x.OneInOneOut(theInput)).Return(expectedOutput);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_have_stored_the_resulting_data_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(expectedOutput));
        }

        [Test]
        public void should_invoke_the_controller_method()
        {
            VerifyCallsFor<ITargetController>();
        }
    }

    [TestFixture]
    public class execute_one_in_zero_out : InteractionContext<OneInZeroOutActionInvoker<ITargetController, Input>>
    {
        private Input theInput;

        protected override void beforeEach()
        {
            Action<ITargetController, Input> action = (x, input) => x.OneInZeroOut(input);
            Services.Inject(action);

            theInput = new Input();

            MockFor<IFubuRequest>().Expect(x => x.Get<Input>()).Return(theInput);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_invoke_the_controller_method()
        {
            MockFor<ITargetController>().AssertWasCalled(x => x.OneInZeroOut(theInput));
        }
    }

    [TestFixture]
    public class execute_zero_in_one_out : InteractionContext<ZeroInOneOutActionInvoker<ITargetController, Output>>
    {
        private Output expectedOutput;

        protected override void beforeEach()
        {
            expectedOutput = new Output();

            Func<ITargetController, Output> action = x => x.ZeroInOneOut();
            Services.Inject(action);

            MockFor<ITargetController>().Expect(x => x.ZeroInOneOut()).Return(expectedOutput);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_execute_the_controller_action()
        {
            VerifyCallsFor<ITargetController>();
        }

        [Test]
        public void should_have_stored_the_resulting_data_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(expectedOutput));
        }
    }


}