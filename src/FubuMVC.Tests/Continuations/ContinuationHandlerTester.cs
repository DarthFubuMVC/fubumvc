using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.Registration;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Continuations
{
    [TestFixture]
    public class ContinuationHandlerTester : InteractionContext<ContinuationHandler>
    {
        private StubUrlRegistry urls;

        protected override void beforeEach()
        {
            urls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(urls);
            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
        }


        [Test]
        public void redirect_with_string_assumes_that_it_is_a_url()
        {
            ClassUnderTest.RedirectTo("some url");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl("some url"));
        }

        [Test]
        public void redirect_with_object_and_category()
        {
            var urlTarget = new UrlTarget();
            ClassUnderTest.RedirectTo(urlTarget, "POST");

            var expectedUrl = urls.UrlFor(urlTarget, "POST");
            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl(expectedUrl));
        }

        [Test]
        public void redirect_to_call_with_category()
        {
            var actionCall = ActionCall.For<FakeEndpoint>(x => x.SayHello());
            ClassUnderTest.RedirectToCall(actionCall, "POST");
            var expectedUrl = urls.UrlFor(typeof (FakeEndpoint), actionCall.Method, "POST");

            MockFor<IOutputWriter>().AssertWasCalled(x => x.RedirectToUrl(expectedUrl));
        }

        [Test]
        public void transfer_to_with_category_and_input_model()
        {
            var foo = new Foo();
            var theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.FindUnique(foo, "POST"))
                .Return(theChain);

            var theBehaviorBeingTransferedTo = MockRepository.GenerateMock<IActionBehavior>();

            MockFor<IPartialFactory>().Stub(x => x.BuildBehavior(theChain)).Return(theBehaviorBeingTransferedTo);


            ClassUnderTest.TransferTo(foo, "POST");

            theBehaviorBeingTransferedTo.AssertWasCalled(x => x.InvokePartial());

        }

        [Test]
        public void transfer_to_with_category_and_action_call()
        {
            var actionCall = ActionCall.For<FakeEndpoint>(x => x.SayHello());

            var theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.Find(actionCall.HandlerType, actionCall.Method, "GET"))
                .Return(theChain);

            var theBehaviorBeingTransferedTo = MockRepository.GenerateMock<IActionBehavior>();

            MockFor<IPartialFactory>().Stub(x => x.BuildBehavior(theChain)).Return(theBehaviorBeingTransferedTo);

            ClassUnderTest.TransferToCall(actionCall, "GET");

            theBehaviorBeingTransferedTo.AssertWasCalled(x => x.InvokePartial());
        }

        public class FakeEndpoint
        {
            public string SayHello()
            {
                return "Hello";
            }
        }

        public class UrlTarget
        {
            
        }
    }

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
    public class finding_the_fubu_continuation : InteractionContext<ContinuationHandler>
    {
        private InMemoryFubuRequest theRequest;
        private FubuContinuation theContinuation;

        protected override void beforeEach()
        {
            theRequest = new InMemoryFubuRequest();

            theContinuation = FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified);
            theRequest.Set(theContinuation);

            Services.Inject<IFubuRequest>(theRequest);
        }

        [Test]
        public void find_the_continuation_if_there_is_a_redirectable()
        {
            var redirectable = new StubRedirectable(){RedirectTo = FubuContinuation.NextBehavior()};
            theRequest.Set(redirectable);

            ClassUnderTest.FindContinuation().ShouldBeTheSameAs(redirectable.RedirectTo);
        }

        [Test]
        public void if_the_redirectable_does_not_have_a_continuation_assume_NextBehavior()
        {
            theRequest.Set(new StubRedirectable{RedirectTo = null});

            ClassUnderTest.FindContinuation().AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void find_the_continuation_without_the_presence_of_an_IRedirectable()
        {
            ClassUnderTest.FindContinuation().ShouldBeTheSameAs(theContinuation);
        }

        public class StubRedirectable : IRedirectable
        {
            public FubuContinuation RedirectTo
            {
                get; set;
            }
        }
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
    public class when_the_continuation_redirects_to_a_destination_object : ContinuationHandlerContext
    {
        private InputModel input;
        private string theUrl;

        protected override void theContextIs()
        {
            input = new InputModel();
            theUrl = "some/path/1";

            MockFor<IUrlRegistry>().Expect(x => x.UrlFor(input, "GET")).Return(theUrl);

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

            MockFor<IUrlRegistry>().Expect(x => x.UrlFor(call.HandlerType, call.Method, "GET")).Return(theUrl);

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
        private BehaviorChain theChain;

        protected override void theContextIs()
        {
            partial = MockRepository.GenerateMock<IActionBehavior>();



            input = new InputModel();
            theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.FindUnique(input)).Return(theChain);

            MockFor<IPartialFactory>().Expect(x => x.BuildBehavior(theChain)).Return(partial);

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
        private BehaviorChain theChain;

        protected override void theContextIs()
        {
            call = ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut());
            partial = MockRepository.GenerateMock<IActionBehavior>();

            theChain = new BehaviorChain();
            MockFor<IChainResolver>().Stub(x => x.Find(call.HandlerType, call.Method)).Return(theChain);

            MockFor<IPartialFactory>().Expect(x => x.BuildBehavior(theChain)).Return(partial);

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