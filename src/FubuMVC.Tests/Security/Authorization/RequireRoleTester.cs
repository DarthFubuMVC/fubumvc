using System.Linq;
using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class RequireRoleTester
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
            new RequireRole("a").RightsFor(null).ShouldBe(AuthorizationRight.Allow);
        }

        [Test]
        public void negative_test_for_a_role_that_is_not_in_the_current_principal()
        {
            new RequireRole("not in principal").RightsFor(null).ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void require_role_attribute()
        {
            var chain = BehaviorChain.For<RequireRoleTester>(x => x.Go());

            chain.Authorization.HasRules().ShouldBeTrue();

            chain.Authorization.Policies.OfType<RequireRole>()
                .Select(x => x.Role).ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [RequireRole("a", "b", "c")]
        public string Go()
        {
            return "hello";
        }
    }


}