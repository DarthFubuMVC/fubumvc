using System.Linq;
using FubuMVC.Authentication.Auditing;
using FubuMVC.Authentication.Cookies;
using FubuMVC.Authentication.Membership;
using FubuMVC.Authentication.Membership.FlatFile;
using FubuMVC.Authentication.Tests.Membership;
using FubuMVC.Authentication.Tickets;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Authentication.Tests
{
    [TestFixture]
    public class AuthenticationServiceRegistryTester
    {
        private ServiceGraph theServiceGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Services<AuthenticationServiceRegistry>();

            theServiceGraph = BehaviorGraph.BuildFrom(registry).Services;
        }

        [Test]
        public void registers_the_LockedOutRule()
        {
            theDefaultServiceIs<ILockedOutRule, LockedOutRule>();
        }

        [Test]
        public void registers_IAuthenticationService()
        {
            theDefaultServiceIs<IAuthenticationService, AuthenticationService>();
        }

        [Test]
        public void registers_the_AuthenticationIsConfigured()
        {
            theServiceGraph.ServicesFor<IActivator>().Select(x => x.Type)
                .ShouldContain(typeof(AuthenticationIsConfigured));
        }

        [Test]
        public void registers_default_IAuthenticationSession()
        {
            theDefaultServiceIs<IAuthenticationSession, TicketAuthenticationSession>();
        }

        [Test]
        public void registers_default_IPrincipalContext()
        {
            theDefaultServiceIs<IPrincipalContext, ThreadPrincipalContext>();
        }

        [Test]
        public void registers_default_ITicketSource()
        {
            theDefaultServiceIs<ITicketSource, CookieTicketSource>();
        }

        [Test]
        public void registers_default_ILoginCookieService()
        {
            theDefaultServiceIs<ILoginCookieService, LoginCookieService>();
        }

        [Test]
        public void registers_default_IEncryptor()
        {
            theDefaultServiceIs<IEncryptor, Encryptor>();
        }

        [Test]
        public void registers_default_ILoginCookies()
        {
            theDefaultServiceIs<ILoginCookies, BasicFubuLoginCookies>();
        }

        [Test]
        public void registers_default_IAuthenticationRedirector()
        {
            theDefaultServiceIs<IAuthenticationRedirector, AuthenticationRedirector>();
        }


        [Test]
        public void registers_default_membership_reposotory()
        {
            theDefaultServiceIs<IMembershipRepository, FlatFileMembershipRepository>();
        }

        [Test]
        public void nullo_auditor_by_default()
        {
            theDefaultServiceIs<ILoginAuditor, NulloLoginAuditor>();
        }

        private void theDefaultServiceIs<TPlugin, TImplementation>()
        {
            theServiceGraph.DefaultServiceFor<TPlugin>()
                .Type.ShouldEqual(typeof(TImplementation));
        }
    }
}