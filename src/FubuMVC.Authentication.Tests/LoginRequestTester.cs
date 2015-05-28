using NUnit.Framework;

using FubuTestingSupport;

namespace FubuMVC.Authentication.Tests
{
    [TestFixture]
    public class LoginRequestTester
    {
        [Test]
        public void status_is_not_authenticated_by_default()
        {
            new LoginRequest().Status.ShouldEqual(LoginStatus.NotAuthenticated);
        }

        [Test]
        public void setting_return_url_sets_url()
        {
            var loginRequest = new LoginRequest();
            var testing_url = "the/best/url";
            loginRequest.ReturnUrl = testing_url;
            loginRequest.Url.ShouldEqual(testing_url);
        }
    }
}