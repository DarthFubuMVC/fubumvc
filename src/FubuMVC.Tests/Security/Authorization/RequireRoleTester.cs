using System.Linq;
using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class RequireRoleTester
    {
        public RequireRoleTester()
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[] { "a", "b" });
            Thread.CurrentPrincipal = principal;
        }

        [Fact]
        public void positive_test_for_a_role_that_is_in_the_current_principal()
        {
            new RequireRole("a").RightsFor(null).ShouldBe(AuthorizationRight.Allow);
        }

        [Fact]
        public void negative_test_for_a_role_that_is_not_in_the_current_principal()
        {
            new RequireRole("not in principal").RightsFor(null).ShouldBe(AuthorizationRight.Deny);
        }

        [Fact]
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