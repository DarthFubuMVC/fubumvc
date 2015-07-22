using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Security.Authentication.Membership.FlatFile;
using FubuMVC.Core.Security.Authentication.Tickets;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class AuthenticationServiceRegistryTester
    {
        [Test]
        public void services_are_registered()
        {
            var registry = new FubuRegistry();
            registry.Features.Authentication.Enable(true);

            using (var runtime = FubuApplication.For(registry).Bootstrap())
            {
                runtime.Container.DefaultRegistrationIs<ILockedOutRule, LockedOutRule>();
                runtime.Container.DefaultRegistrationIs<IAuthenticationService, AuthenticationService>();
                runtime.Container.ShouldHaveRegistration<IActivator, AuthenticationIsConfigured>();

                runtime.Container.DefaultRegistrationIs<IAuthenticationSession, TicketAuthenticationSession>();
                runtime.Container.DefaultRegistrationIs<IPrincipalContext, ThreadPrincipalContext>();
                runtime.Container.DefaultRegistrationIs<ITicketSource, CookieTicketSource>();
                runtime.Container.DefaultRegistrationIs<ILoginCookieService, LoginCookieService>();
                runtime.Container.DefaultRegistrationIs<IEncryptor, Encryptor>();
                runtime.Container.DefaultRegistrationIs<ILoginCookies, BasicFubuLoginCookies>();
                runtime.Container.DefaultRegistrationIs<IAuthenticationRedirector, AuthenticationRedirector>();
                runtime.Container.DefaultRegistrationIs<IMembershipRepository, FlatFileMembershipRepository>();
                runtime.Container.DefaultRegistrationIs<ILoginAuditor, NulloLoginAuditor>();
            }
        }
    }
}