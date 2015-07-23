using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class when_authenticating_and_authentication_is_disabled : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;

        protected override void beforeEach()
        {
            Services.Inject(new SecuritySettings
            {
                AuthenticationEnabled = false
            });

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue()
        {
            theResult.AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void should_not_actually_do_authentication()
        {
            MockFor<IAuthenticationService>().AssertWasNotCalled(x => x.TryToApply());
        }
    }


    [TestFixture]
    public class when_authenticating_and_there_is_not_a_previous_authentication_token : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;
        private FubuContinuation theRedirect;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);

            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(new AuthResult{Success = false});

            theRedirect = FubuContinuation.RedirectTo<LoginRequest>();

            MockFor<IAuthenticationRedirector>().Stub(x => x.Redirect())
                                                .Return(theRedirect);

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_redirect_based_on_what_IAuthenticationRedirector_decides()
        {
            theResult.ShouldBeTheSameAs(theRedirect);
        }
    }

    [TestFixture]
    public class when_authenticating_and_there_is_a_previously_authenticated_user : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);
            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(new AuthResult{Success = true});

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue()
        {
			theResult.AssertWasContinuedToNextBehavior();
        }

    }

    [TestFixture]
    public class when_authenticating_and_the_service_returns_a_continuation_on_success : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;
        private FubuContinuation theContinuation;

        protected override void beforeEach()
        {
            theContinuation = FubuContinuation.RedirectTo("somewhere");

            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);
            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(new AuthResult { Success = true, Continuation = theContinuation});

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue_to_what_the_service_determined()
        {
            theResult.ShouldBeTheSameAs(theContinuation);
        }
    }

    [TestFixture]
    public class when_authenticating_and_the_service_returns_a_continuation_on_failure : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;
        private FubuContinuation theContinuation;

        protected override void beforeEach()
        {
            theContinuation = FubuContinuation.RedirectTo("somewhere");

            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);
            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(new AuthResult { Success = false, Continuation = theContinuation });

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue_to_what_the_service_determined()
        {
            theResult.ShouldBeTheSameAs(theContinuation);
        }
    }

    [TestFixture]
    public class when_authenticating_in_a_partial_always_go_to_next : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(true);

            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(new AuthResult{Success = false});

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue()
        {
            theResult.AssertWasContinuedToNextBehavior();
        }
    }
}