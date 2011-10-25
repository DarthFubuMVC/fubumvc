using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Continuations
{
    public abstract class ContinuationHandlerContext : InteractionContext<ContinuationHandler>
    {
        protected IActionBehavior theInsideBehavior;

        protected void ProcessContinuation(FubuContinuation continuation)
        {
            continuation.Process(ClassUnderTest);
        }

        protected override sealed void beforeEach()
        {
            theInsideBehavior = MockRepository.GenerateMock<IActionBehavior>();
            ClassUnderTest.InsideBehavior = theInsideBehavior;

            theContextIs();
        }

        protected abstract void theContextIs();
    }

    [TestFixture]
    public class when_ending_with_a_status_code : ContinuationHandlerContext
    {
        protected override void theContextIs()
        {
            ClassUnderTest.EndWithStatusCode(HttpStatusCode.NotModified);
        }

        [Test]
        public void the_inside_behavior_should_not_be_invoked()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void the_status_code_should_be_written_to_the_output_writer()
        {
            MockFor<IOutputWriter>().WriteResponseCode(HttpStatusCode.NotModified);
        }
    }


    [TestFixture]
    public class when_invoking_the_next_behavior : ContinuationHandlerContext
    {
        protected override void theContextIs()
        {
            ProcessContinuation(FubuContinuation.NextBehavior());
        }

        [Test]
        public void should_invoke_the_inside_behavior()
        {
            theInsideBehavior.AssertWasCalled(x => x.Invoke());
        }
    }


    [TestFixture]
    public class when_the_continuation_redirects_to_a_destination_object : ContinuationHandlerContext
    {
        private InputModel input;
        private string theUrl;

        protected override void theContextIs()
        {
            input = new InputModel();
            theUrl = "some/path/1";

            MockFor<IUrlRegistry>().Expect(x => x.UrlFor(input)).Return(theUrl);

            ProcessContinuation(FubuContinuation.RedirectTo(input));
        }

        [Test]
        public void redirect_to_the_url_from_url_registry()
        {
            VerifyCallsFor<IUrlRegistry>();
            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl(theUrl));
        }

        [Test]
        public void should_not_invoke_the_inner_behavior()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_the_continuation_redirects_to_a_string : ContinuationHandlerContext
    {
        private string theUrl;

        protected override void theContextIs()
        {
            theUrl = "some/path/1";

            ProcessContinuation(FubuContinuation.RedirectTo(theUrl));
        }

        [Test]
        public void invokes_output_writer_with_the_new_url()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl(theUrl));
        }

        [Test]
        public void should_not_invoke_the_inner_behavior()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_the_continuation_redirects_to_an_action_call : ContinuationHandlerContext
    {
        private string theUrl;
        private ActionCall call;

        protected override void theContextIs()
        {
            theUrl = "some/path/1";
            call = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());

            MockFor<IUrlRegistry>().Expect(x => x.UrlFor(call.HandlerType, call.Method)).Return(theUrl);

            ProcessContinuation(FubuContinuation.RedirectTo<ControllerTarget>(x => x.ZeroInOneOut()));
        }


        [Test]
        public void should_not_invoke_the_inner_behavior()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_redirect_to_the_url_for_the_specified_continuation()
        {
            VerifyCallsFor<IUrlRegistry>();
            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl(theUrl));
        }
    }


    [TestFixture]
    public class when_transferring_to_a_destination_object : ContinuationHandlerContext
    {
        private IActionBehavior partial;
        private InputModel input;

        protected override void theContextIs()
        {
            partial = MockRepository.GenerateMock<IActionBehavior>();
            input = new InputModel();

            MockFor<IPartialFactory>().Expect(x => x.BuildPartial(typeof (InputModel))).Return(partial);

            ProcessContinuation(FubuContinuation.TransferTo(input));
        }

        [Test]
        public void find_the_partial_behavior_by_the_destination_object()
        {
            VerifyCallsFor<IPartialFactory>();
        }

        [Test]
        public void should_invoke_the_partial_behavior()
        {
            partial.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_not_invoke_the_inner_behavior()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_register_the_destination_object_into_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.SetObject(input));
        }
    }

    [TestFixture]
    public class when_transferring_to_an_action_call : ContinuationHandlerContext
    {
        private ActionCall call;
        private IActionBehavior partial;

        protected override void theContextIs()
        {
            call = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            partial = MockRepository.GenerateMock<IActionBehavior>();

            MockFor<IPartialFactory>().Expect(x => x.BuildPartial(call)).Return(partial);

            ProcessContinuation(FubuContinuation.TransferTo<ControllerTarget>(x => x.ZeroInOneOut()));
        }


        [Test]
        public void find_the_partial_behavior_by_the_destination_object()
        {
            VerifyCallsFor<IPartialFactory>();
        }

        [Test]
        public void should_invoke_the_partial_behavior()
        {
            partial.AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void should_not_invoke_the_inner_behavior()
        {
            theInsideBehavior.AssertWasNotCalled(x => x.Invoke());
        }
    }
}