using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class LoginRequestTester
    {
        [Fact]
        public void status_is_not_authenticated_by_default()
        {
            new LoginRequest().Status.ShouldBe(LoginStatus.NotAuthenticated);
        }

        [Fact]
        public void setting_return_url_sets_url()
        {
            var loginRequest = new LoginRequest();
            var testing_url = "the/best/url";
            loginRequest.ReturnUrl = testing_url;
            loginRequest.Url.ShouldBe(testing_url);
        }
    }
}