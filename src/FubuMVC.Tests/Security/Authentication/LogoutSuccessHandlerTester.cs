using FubuMVC.Core.Security.Authentication;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class LogoutSuccessHandlerTester
    {
        [Test]
        public void should_redirect_to_the_login_page()
        {
            new LogoutSuccessHandler().LoggedOut()
                .AssertWasRedirectedTo(new LoginRequest(), "GET");
        }
    }
}