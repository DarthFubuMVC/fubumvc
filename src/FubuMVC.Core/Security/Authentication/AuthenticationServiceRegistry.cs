using System.Web.Caching;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authentication.Tickets;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationServiceRegistry : ServiceRegistry
    {
        public AuthenticationServiceRegistry()
        {
            SetServiceIfNone<IAuthenticationSession, TicketAuthenticationSession>();

            // TODO -- Break out the "Basic" stuff into a separate registry
            SetServiceIfNone<IPrincipalContext, ThreadPrincipalContext>();
            SetServiceIfNone<ITicketSource, CookieTicketSource>();
            SetServiceIfNone<IEncryptor, Encryptor>();
            SetServiceIfNone<ILoginCookies, BasicFubuLoginCookies>();
            SetServiceIfNone<ILoginCookieService, LoginCookieService>();
            SetServiceIfNone<ILoginSuccessHandler, LoginSuccessHandler>();
            SetServiceIfNone<ILogoutSuccessHandler, LogoutSuccessHandler>();
            SetServiceIfNone<IAuthenticationRedirector, AuthenticationRedirector>();
            SetServiceIfNone<IAuthenticationService, AuthenticationService>();
            SetServiceIfNone<IPasswordHash, PasswordHash>();
            SetServiceIfNone<ILockedOutRule, LockedOutRule>();

            AddService<IActivator, AuthenticationIsConfigured>();

            SetServiceIfNone<ILoginAuditor, NulloLoginAuditor>();

            SetServiceIfNone<IMembershipRepository, InMemoryMembershipRepository>().Singleton();
        }
    }


}