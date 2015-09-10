using FubuMVC.Core;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authentication.Tickets;
using NUnit.Framework;
using StructureMap;

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

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();
                container.DefaultRegistrationIs<ILockedOutRule, LockedOutRule>();
                container.DefaultRegistrationIs<IAuthenticationService, AuthenticationService>();
                container.ShouldHaveRegistration<IActivator, AuthenticationIsConfigured>();

                container.DefaultRegistrationIs<IAuthenticationSession, TicketAuthenticationSession>();
                container.DefaultRegistrationIs<IPrincipalContext, ThreadPrincipalContext>();
                container.DefaultRegistrationIs<ITicketSource, CookieTicketSource>();
                container.DefaultRegistrationIs<ILoginCookieService, LoginCookieService>();
                container.DefaultRegistrationIs<IEncryptor, Encryptor>();
                container.DefaultRegistrationIs<ILoginCookies, BasicFubuLoginCookies>();
                container.DefaultRegistrationIs<IAuthenticationRedirector, AuthenticationRedirector>();
                container.DefaultRegistrationIs<ILoginAuditor, NulloLoginAuditor>();
            }
        }
    }
}