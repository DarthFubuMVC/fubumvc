using NUnit.Framework;

namespace FubuMVC.Authentication.Tests
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