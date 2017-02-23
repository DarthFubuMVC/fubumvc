using FubuMVC.Core.Security.Authentication;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class LogoutSuccessHandlerTester
    {
        [Fact]
        public void should_redirect_to_the_login_page()
        {
            new LogoutSuccessHandler().LoggedOut()
                .AssertWasRedirectedTo(new LoginRequest(), "GET");
        }
    }
}