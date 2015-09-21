using System.Linq;
using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;
using Rhino.Mocks;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class AllowRoleTester
    {
        [SetUp]
        public void SetUp()
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[] { "a", "b" });
            Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void positive_test_for_a_role_that_is_in_the_current_principal()
        {
            new AllowRole("a").RightsFor(null).ShouldBe(AuthorizationRight.Allow);
        }

        [Test]
        public void negative_test_for_a_role_that_is_not_in_the_current_principal()
        {
            new AllowRole("not in principal").RightsFor(null).ShouldBe(AuthorizationRight.None);
        }
    }

    [TestFixture]
    public class MustBeAuthenticatedTester
    {
        [Test]
        public void no_principal_no_authorization()
        {
            MustBeAuthenticated.DetermineRights(null).ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void not_authenticated_principal()
        {
            var principal = MockRepository.GenerateMock<IPrincipal>();
            var identity = MockRepository.GenerateMock<IIdentity>();
            identity.Stub(x => x.IsAuthenticated).Return(false);
            principal.Stub(x => x.Identity).Return(identity);

            MustBeAuthenticated.DetermineRights(principal).ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void is_authenticated()
        {
            var principal = MockRepository.GenerateMock<IPrincipal>();
            var identity = MockRepository.GenerateMock<IIdentity>();
            identity.Stub(x => x.IsAuthenticated).Return(true);
            principal.Stub(x => x.Identity).Return(identity);

            MustBeAuthenticated.DetermineRights(principal).ShouldBe(AuthorizationRight.Allow);
        }

        [Test]
        public void attribute_places_the_rule()
        {
            var chain = BehaviorChain.For<MustBeAuthenticatedTester>(x => x.Go());

            chain.Authorization.Policies.Single()
                .ShouldBeOfType<MustBeAuthenticated>();
        }

        [MustBeAuthenticated]
        public string Go()
        {
            return "ok";
        }
    }
}