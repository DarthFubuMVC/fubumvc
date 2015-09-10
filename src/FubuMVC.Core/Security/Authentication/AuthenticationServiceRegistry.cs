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

            SetServiceIfNone<IMembershipRepository, Membership.InMemoryMembershipRepository>().Singleton();
        }
    }

    public class InMemoryMembershipRepository : IMembershipRepository
    {
        private readonly IPasswordHash _hash;
        private readonly Cache<string, string> _credentials = new Cache<string, string>();




        public InMemoryMembershipRepository(IPasswordHash hash)
        {
            _hash = hash;
        }

        public void StoreCredentials(string user, string password)
        {
            _credentials[user] = _hash.CreateHash(password);
        }

        public bool MatchesCredentials(LoginRequest request)
        {
            if (!_credentials.Has(request.UserName)) return false;

            var hash = _hash.CreateHash(request.Password);
            return _credentials[request.UserName] == hash;
        }

        public IUserInfo FindByName(string username)
        {
            if (!_credentials.Has(username)) return null;

            return new UserInfo{UserName = username};
        }
    }
}