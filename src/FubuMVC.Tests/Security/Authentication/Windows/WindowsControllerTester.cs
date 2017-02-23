using System.Net;
using System.Security.Principal;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication.Windows;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    
    public class WindowsControllerTester : InteractionContext<WindowsController>
    {
        private WindowsPrincipal thePrincipal;
        private FubuContinuation theContinuation;
        private WindowsSignInRequest theRequest;

        protected override void beforeEach()
        {
            thePrincipal = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
            theContinuation = FubuContinuation.EndWithStatusCode(HttpStatusCode.Accepted);
            theRequest = new WindowsSignInRequest();

            MockFor<IWindowsAuthenticationContext>().Stub(x => x.Current()).Return(thePrincipal);
            MockFor<IWindowsAuthentication>().Stub(x => x.Authenticate(theRequest, thePrincipal)).Return(theContinuation);
        }

        [Fact]
        public void just_delegates_to_the_strategy()
        {
            ClassUnderTest.Login(theRequest).ShouldBe(theContinuation);
        }
    }
}