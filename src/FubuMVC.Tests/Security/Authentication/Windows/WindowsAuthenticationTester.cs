using System.Security.Principal;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Windows;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    
    public class when_the_windows_authentication_is_successful : InteractionContext<WindowsAuthentication>
    {
        private FubuContinuation theContinuation;
        private WindowsPrincipal thePrincipal;
        private WindowsSignInRequest theRequest;

        protected override void beforeEach()
        {
            thePrincipal = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
            theRequest = new WindowsSignInRequest();

            MockFor<IWindowsPrincipalHandler>().Stub(x => x.Authenticated(thePrincipal))
                                               .Return(true);

            theContinuation = ClassUnderTest.Authenticate(theRequest, thePrincipal);
        }

        [Fact]
        public void should_be_auditing_the_success()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(new SuccessfulWindowsAuthentication{User = thePrincipal.Identity.Name}));
        }

        [Fact]
        public void invokes_the_principal_handler()
        {
            MockFor<IWindowsPrincipalHandler>().AssertWasCalled(x => x.Authenticated(thePrincipal));
        }

        [Fact]
        public void redirects_the_redirect_url()
        {
            theContinuation.AssertWasRedirectedTo(theRequest.Url);
        }

        [Fact]
        public void marks_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAuthenticated(thePrincipal.Identity.Name));
        }
    }


    
    public class when_the_windows_authentication_fails : InteractionContext<WindowsAuthentication>
    {
        private FubuContinuation theContinuation;
        private WindowsPrincipal thePrincipal;
        private WindowsSignInRequest theRequest;

        protected override void beforeEach()
        {
            thePrincipal = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
            theRequest = new WindowsSignInRequest
            {
                Url = "some url"
            };

            MockFor<IWindowsPrincipalHandler>().Stub(x => x.Authenticated(thePrincipal))
                                               .Return(false);

            theContinuation = ClassUnderTest.Authenticate(theRequest, thePrincipal);
        }

        [Fact]
        public void should_be_auditing_the_failure()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(new FailedWindowsAuthentication { User = thePrincipal.Identity.Name }));
        }

        [Fact]
        public void invokes_the_principal_handler()
        {
            MockFor<IWindowsPrincipalHandler>().AssertWasCalled(x => x.Authenticated(thePrincipal));
        }

        [Fact]
        public void should_redirect_to_the_login_page()
        {
            theContinuation.AssertWasRedirectedTo(new LoginRequest{Url = theRequest.Url}, "GET");
        }

        [Fact]
        public void DOES_NOT_marks_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasNotCalled(x => x.MarkAuthenticated(thePrincipal.Identity.Name));
        }
    }



}