using System.Security.Principal;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class when_successfully_trying_to_apply_authentication : InteractionContext<BasicAuthentication>
    {
        private string theUserName;
        private IPrincipal thePrincipal;
        private AuthResult theResult;

        protected override void beforeEach()
        {
            theUserName = "a user";

            MockFor<IAuthenticationSession>().Stub(x => x.PreviouslyAuthenticatedUser())
                                             .Return(theUserName);

            thePrincipal = MockFor<IPrincipal>();


            MockFor<IPrincipalBuilder>().Stub(x => x.Build(theUserName))
                                        .Return(thePrincipal);

            theResult = ClassUnderTest.TryToApply();
        }

        [Test]
        public void should_mark_the_session_as_accessed_for_sliding_expirations()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAccessed());
        }

        [Test]
        public void should_set_the_principal_for_the_authenticated_user()
        {
            MockFor<IPrincipalContext>().AssertWasCalled(x => x.Current = thePrincipal);
        }

        [Test]
        public void was_successful()
        {
            theResult.Success.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_unsuccessfully_trying_to_apply_authentication : InteractionContext<BasicAuthentication>
    {
        private AuthResult theResult;

        protected override void beforeEach()
        {
            MockFor<IAuthenticationSession>().Stub(x => x.PreviouslyAuthenticatedUser())
                                             .Return(null);

            theResult = ClassUnderTest.TryToApply();
        }

        [Test]
        public void should_return_false_because_no_user_is_previously_authenticated()
        {
            theResult.Success.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_the_authentication_fails : InteractionContext<BasicAuthentication>
    {
        private LoginRequest theLoginRequest;
        private bool theResult;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go",
                NumberOfTries = 2
            };

            MockFor<ICredentialsAuthenticator>().Stub(x => x.AuthenticateCredentials(theLoginRequest))
                                                .Return(false);

            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theLoginRequest)).Return(LoginStatus.NotAuthenticated);

            theResult = ClassUnderTest.Authenticate(theLoginRequest);
        }

        [Test]
        public void should_NOT_mark_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasNotCalled(x => x.MarkAuthenticated(theLoginRequest.UserName));
        }

        [Test]
        public void should_increment_the_number_of_retries()
        {
            theLoginRequest.NumberOfTries.ShouldBe(3);
        }

        [Test]
        public void should_mark_the_login_request_as_failed()
        {
            theLoginRequest.Status.ShouldBe(LoginStatus.Failed);
        }

        [Test]
        public void the_result_was_not_successful()
        {
            theResult.ShouldBeFalse();
        }
    }


    [TestFixture]
    public class when_the_authentication_fails_because_the_user_is_locked_out : InteractionContext<BasicAuthentication>
    {
        private LoginRequest theLoginRequest;
        private bool theResult;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go",
                NumberOfTries = 2
            };

            MockFor<ICredentialsAuthenticator>().Stub(x => x.AuthenticateCredentials(theLoginRequest))
                                                .Return(false);

            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theLoginRequest)).Return(LoginStatus.LockedOut);

            theResult = ClassUnderTest.Authenticate(theLoginRequest);
        }

        [Test]
        public void should_NOT_mark_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasNotCalled(x => x.MarkAuthenticated(theLoginRequest.UserName));
        }

        [Test]
        public void should_not_increment_the_number_of_retries()
        {
            theLoginRequest.NumberOfTries.ShouldBe(2);
        }

        [Test]
        public void should_mark_the_login_request_as_locked_out()
        {
            theLoginRequest.Status.ShouldBe(LoginStatus.LockedOut);
        }

        [Test]
        public void the_result_was_not_successful()
        {
            theResult.ShouldBeFalse();
        }
    }


    [TestFixture]
    public class when_authentication_succeeds : InteractionContext<BasicAuthentication>
    {
        private LoginRequest theLoginRequest;
        private bool theResult;

        protected override void beforeEach()
        {
            theLoginRequest = new LoginRequest
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go"
            };

            MockFor<ICredentialsAuthenticator>().Stub(x => x.AuthenticateCredentials(theLoginRequest))
                                                .Return(true);

            MockFor<ILockedOutRule>().Stub(x => x.IsLockedOut(theLoginRequest)).Return(LoginStatus.NotAuthenticated);

            theResult = ClassUnderTest.Authenticate(theLoginRequest);
        }


        [Test]
        public void does_not_check_for_lockout_on_success()
        {
            MockFor<ILockedOutRule>().AssertWasNotCalled(x => x.ProcessFailure(theLoginRequest));
        }


        [Test]
        public void should_mark_the_login_request_as_successful()
        {
            theLoginRequest.Status.ShouldBe(LoginStatus.Succeeded);
        }

        [Test]
        public void should_mark_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAuthenticated(theLoginRequest.UserName));
        }

        [Test]
        public void the_return_value_should_be_true()
        {
            theResult.ShouldBeTrue();
        }
    }
}