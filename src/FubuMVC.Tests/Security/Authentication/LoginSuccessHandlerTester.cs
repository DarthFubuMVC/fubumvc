using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class LoginSuccessHandlerTester : InteractionContext<LoginSuccessHandler>
    {
        private LoginRequest theLoginRequest;

        protected override void beforeEach()
        {
            MockFor<IHttpRequest>().Stub(x => x.HttpMethod()).Return("POST");

            theLoginRequest = new LoginRequest()
            {
                UserName = "frank",
                Url = "/where/i/wanted/to/go"
            };

            
        }

        [Fact]
        public void should_redirect_the_browser_to_the_original_url()
        {
            ClassUnderTest.LoggedIn(theLoginRequest).AssertWasRedirectedTo(theLoginRequest.Url);
        }
    }
}