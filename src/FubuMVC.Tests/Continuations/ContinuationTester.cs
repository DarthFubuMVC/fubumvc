using System;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Registration;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Continuations
{
    [TestFixture]
    public class ContinuationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            director = MockRepository.GenerateMock<IContinuationDirector>();
        }

        #endregion

        private IContinuationDirector director;

        private void shouldFail(Action action)
        {
            Exception<FubuAssertionException>.ShouldBeThrownBy(action);
        }


        [Test]
        public void assert_continued()
        {
            FubuContinuation continuation = FubuContinuation.NextBehavior();
            continuation.AssertWasContinuedToNextBehavior();

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_redirect_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null));
            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null));

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());
        }

        [Test]
        public void assert_redirect_to_a_target()
        {
            var input = new InputModelWithEquals{Name = "Luke"};
            FubuContinuation continuation = FubuContinuation.RedirectTo(input);

            continuation.AssertWasRedirectedTo(new InputModelWithEquals {Name = "Luke"});

            shouldFail(() => continuation.AssertWasRedirectedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasTransferedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_transfer_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null));

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());
        }


        [Test]
        public void assert_transfer_to_a_target()
        {
            var input = new InputModelWithEquals{Name="Luke"};
            FubuContinuation continuation = FubuContinuation.TransferTo(input);

            continuation.AssertWasTransferedTo(new InputModelWithEquals { Name = "Luke" });

            shouldFail(() => continuation.AssertWasTransferedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasRedirectedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void Continue_just_continues()
        {
            FubuContinuation continuation = FubuContinuation.NextBehavior();
            continuation.Type.ShouldEqual(ContinuationType.NextBehavior);

            continuation.Process(director);
            director.AssertWasCalled(x => x.InvokeNextBehavior());
        }

        [Test]
        public void redirect_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.RedirectToCall(call));
        }

        [Test]
        public void redirect_to_a_target()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.RedirectTo(input);

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            director.AssertWasCalled(x => x.RedirectTo(input));
        }


        [Test]
        public void transfer_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.TransferToCall(call));
        }

        [Test]
        public void transfer_to_a_target()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.TransferTo(input);

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            director.AssertWasCalled(x => x.TransferTo(input));
        }
    }
}