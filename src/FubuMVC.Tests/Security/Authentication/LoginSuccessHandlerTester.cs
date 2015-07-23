using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
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

        [Test]
        public void should_redirect_the_browser_to_the_original_url()
        {
            ClassUnderTest.LoggedIn(theLoginRequest).AssertWasRedirectedTo(theLoginRequest.Url);
        }
    }
}