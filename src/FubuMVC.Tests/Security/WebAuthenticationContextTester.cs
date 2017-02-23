using FubuMVC.Core.Security;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Web.Security
{
    
    public class WebAuthenticationContextTester
    {
        public WebAuthenticationContextTester()
        {
            _actualUser = null;
            _actualRememberMe = null;
            _signOutCalled = false;

            _authSvc = new WebAuthenticationContext
            {
                SetAuthCookieFunc = ((u, r) =>
                {
                    _actualUser = u;
                    _actualRememberMe = r;
                }),
                SignOutFunc = () => { _signOutCalled = true; }
            };
        }


        private WebAuthenticationContext _authSvc;
        private string _actualUser;
        private bool? _actualRememberMe;
        private bool _signOutCalled;

        [Fact]
        public void should_set_the_forms_auth_token_with_the_rememberMe_setting()
        {
            string username = "user";

            _authSvc.ThisUserHasBeenAuthenticated(username, true);

            _actualUser.ShouldBe(username);
            _actualRememberMe.Value.ShouldBeTrue();
        }

        [Fact]
        public void SignOut_should_signout_from_forms_auth()
        {
            _authSvc.SignOut();

            _signOutCalled.ShouldBeTrue();
        }
    }
}