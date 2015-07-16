using FubuMVC.Authentication.Auditing;
using FubuMVC.Authentication.Cookies;
using FubuMVC.Authentication.Membership;
using FubuMVC.Authentication.Membership.FlatFile;
using FubuMVC.Authentication.Tickets;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.Authentication
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

            SetServiceIfNone<IMembershipRepository, FlatFileMembershipRepository>();
            SetServiceIfNone<ILoginAuditor, NulloLoginAuditor>();
        }
    }
}