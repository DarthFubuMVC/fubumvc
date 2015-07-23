using System;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Endpoints
{
    public class StubLoginCookies : ILoginCookies
    {
        private readonly ICookieValue _user = MockRepository.GenerateMock<ICookieValue>();

        public ICookieValue User
        {
            get { return _user; }
        }
    }

    [TestFixture]
    public class when_going_to_the_login_screen_with_a_remembered_user : InteractionContext<LoginController>
    {
        private StubLoginCookies theCookies;

        protected override void beforeEach()
        {
            theCookies = new StubLoginCookies();
            Services.Inject<ILoginCookies>(theCookies);

            this.SetupAuthenticationService();
        }

        [Test]
        public void does_nothing_if_there_is_no_remembered_user()
        {
            var request = new LoginRequest
            {
                UserName = null
            };

            theCookies.User.Stub(x => x.Value).Return(null);

            ClassUnderTest.get_login(request);

            request.UserName.ShouldBeNull();
            request.RememberMe.ShouldBeFalse();
        }

        [Test]
        public void should_not_set_the_remembered_user_name_if_there_is_already_a_different_name()
        {
            var request = new LoginRequest
            {
                UserName = "josh"
            };

            theCookies.User.Stub(x => x.Value).Return("jeremy");

            ClassUnderTest.get_login(request);

            request.UserName.ShouldBe("josh");
            request.RememberMe.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_remembered_user_name_on_the_login_request_if_it_is_not_already_set()
        {
            var request = new LoginRequest
            {
                UserName = null
            };

            theCookies.User.Stub(x => x.Value).Return("jeremy");

            ClassUnderTest.get_login(request);

            request.UserName.ShouldBe("jeremy");
            request.RememberMe.ShouldBeTrue();
        }

        [Test]
        public void UserName_should_set_itself_on_the_post_route_if_remember_me_is_true()
        {
            const string username = "fakeuser";
            var request = new LoginRequest
            {
                UserName = username,
                RememberMe = true
            };

            ClassUnderTest.post_login(request);

            theCookies.User.AssertWasCalled(x => x.Value = username);
            request.UserName.ShouldBe(username);
        }

        [Test]
        public void UserName_should_not_set_itself_on_the_post_route_if_remember_me_is_false()
        {
            const string username = "fakeuser";
            var request = new LoginRequest
            {
                UserName = username,
                RememberMe = false
            };

            ClassUnderTest.post_login(request);

            theCookies.User.AssertWasNotCalled(x => x.Value = username);
            theCookies.User.Value.ShouldNotBe(username);
        }
    }

    [TestFixture]
    public class LoginControllerTester : InteractionContext<LoginController>
    {
        private LoginRequest theRequest;
        private AuthenticationSettings theSettings;

        protected override void beforeEach()
        {
            LocalSystemTime = DateTime.Today.AddHours(10);

            theSettings = new AuthenticationSettings();
            Services.Inject(theSettings);

            theRequest = new LoginRequest();
            Services.Inject<ILoginCookies>(new StubLoginCookies());
        }


        [Test]
        public void show_initial_screen_not_logged_out()
        {
            var request = new LoginRequest{Status = LoginStatus.NotAuthenticated};

            ClassUnderTest.get_login(request).ShouldBeTheSameAs(request);

            request.Message.ShouldBeNull();
        }

        [Test]
        public void show_initial_screen_when_the_user_is_locked_out()
        {
            var request = new LoginRequest();

            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(request)).Return(LoginStatus.LockedOut);

            ClassUnderTest.get_login(request);

            request.Message.ShouldBe(LoginKeys.LockedOut.ToString());
        }


        [Test]
        public void uses_the_unknown_message_when_no_message_is_set()
        {
            theRequest.Status = LoginStatus.Failed;

            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theRequest)).Return(LoginStatus.NotAuthenticated);


            ClassUnderTest.get_login(theRequest);

            theRequest.Message.ShouldBe(LoginKeys.Unknown.ToString());
        }

        [Test]
        public void leave_the_existing_message_if_failed()
        {
            theRequest.Status = LoginStatus.Failed;
            theRequest.Message = "Something bad";

            ClassUnderTest.get_login(theRequest);

            theRequest.Message.ShouldBe("Something bad");
        }
    }


    [TestFixture]
    public class when_running_in_a_get_that_is_not_locked_out : InteractionContext<LoginController>
    {
        private LoginRequest theLoginRequest;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest();

            MockFor<IHttpRequest>().Stub(x => x.HttpMethod()).Return("GET");
            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theLoginRequest)).Return(LoginStatus.NotAuthenticated);
            MockFor<IFubuRequest>().Stub(x => x.Get<LoginRequest>()).Return(theLoginRequest);

            Services.Inject<ILoginCookies>(new StubLoginCookies());

            ClassUnderTest.get_login(theLoginRequest);
        }

        [Test]
        public void the_status_is_still_not_authenticated()
        {
            theLoginRequest.Status.ShouldBe(LoginStatus.NotAuthenticated);
        }

        [Test]
        public void should_not_even_try_to_authenticate()
        {
            MockFor<IAuthenticationService>().AssertWasNotCalled(x => x.Authenticate(null), x => x.IgnoreArguments());
        }


    }

    [TestFixture]
    public class when_running_in_a_get_that_is_locked_out : InteractionContext<LoginController>
    {
        private LoginRequest theLoginRequest;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest();

            MockFor<IHttpRequest>().Stub(x => x.HttpMethod()).Return("GET");
            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theLoginRequest))
                .Return(LoginStatus.LockedOut);

            Services.Inject<ILoginCookies>(new StubLoginCookies());

            MockFor<IFubuRequest>().Stub(x => x.Get<LoginRequest>()).Return(theLoginRequest);

            ClassUnderTest.get_login(theLoginRequest);
        }

        [Test]
        public void the_status_should_be_locked_out()
        {
            theLoginRequest.Status.ShouldBe(LoginStatus.LockedOut);
        }

        [Test]
        public void should_not_even_try_to_authenticate()
        {
            MockFor<IAuthenticationService>().AssertWasNotCalled(x => x.Authenticate(null), x => x.IgnoreArguments());
        }


    }

    [TestFixture]
    public class when_successfully_authenticating : InteractionContext<LoginController>
    {
        private LoginRequest theLoginRequest;
        private FubuContinuation theContinuation;
        private FubuContinuation successfulContinuation;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest()
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go"
            };
            MockFor<IFubuRequest>().Stub(x => x.Get<LoginRequest>()).Return(theLoginRequest);

            this.SetupAuthenticationService();

            successfulContinuation = FubuContinuation.RedirectTo("something");
            MockFor<ILoginSuccessHandler>().Stub(x => x.LoggedIn(theLoginRequest)).Return(successfulContinuation);

            Services.Inject<ILoginCookies>(new StubLoginCookies());

            theContinuation = ClassUnderTest.post_login(theLoginRequest);
        }

        [Test]
        public void should_have_applied_history()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.ApplyHistory(theLoginRequest));
        }

        [Test]
        public void should_audit_the_request()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(theLoginRequest));
        }

        [Test]
        public void should_not_allow_the_inner_behavior_to_execute()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_use_the_continuation_from_the_success_handler()
        {
            theContinuation.ShouldBeTheSameAs(successfulContinuation);
        }
    }

    [TestFixture]
    public class when_UNsuccessfully_authenticating : InteractionContext<LoginController>
    {
        private LoginRequest theLoginRequest;
        private FubuContinuation theContinuation;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest()
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go",
                NumberOfTries = 2
            };
            
            MockFor<IFubuRequest>().Stub(x => x.Get<LoginRequest>()).Return(theLoginRequest);

            this.SetupAuthenticationService(false);

            theContinuation = ClassUnderTest.post_login(theLoginRequest);
        }

        [Test]
        public void should_have_applied_history()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.ApplyHistory(theLoginRequest));
        }


        [Test]
        public void should_audit_the_request()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(theLoginRequest));
        }

        [Test]
        public void should_not_signal_the_success_handler()
        {
            MockFor<ILoginSuccessHandler>().AssertWasNotCalled(x => x.LoggedIn(theLoginRequest));
        }

        [Test]
        public void should_proceed_to_the_login_page()
        {
            theContinuation.AssertWasTransferedTo(theLoginRequest, "GET");
        }
    }

    public static class AuthenticationServiceHelper {

        public static void SetupAuthenticationService(this InteractionContext<LoginController> context, bool authenticates = true)
        {
            var strategies = context.Services.CreateMockArrayFor<IAuthenticationStrategy>(1);
            strategies[0].Stub(x => x.Authenticate(Arg<LoginRequest>.Is.Anything)).Return(authenticates);
            var authService = new AuthenticationService(
                context.MockFor<ILogger>(), 
                strategies, 
                context.MockFor<ILoginAuditor>(),
                context.MockFor<ILoginCookies>());
            context.Services.Inject(authService as IAuthenticationService);
        }

    }
}