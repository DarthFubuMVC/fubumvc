using System;
using System.Web.Security;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Web.Security
{
    public class WebAuthenticationContext : IAuthenticationContext
    {
        public Action<string, bool> SetAuthCookieFunc = FormsAuthentication.SetAuthCookie;
        public Action SignOutFunc = FormsAuthentication.SignOut;


        public void ThisUserHasBeenAuthenticated(string username, bool rememberMe)
        {
            SetAuthCookieFunc(username, rememberMe);
        }

        public void SignOut()
        {
            SignOutFunc();
        }
    }
}