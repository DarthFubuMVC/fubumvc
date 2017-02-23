using System.Threading;
using FubuMVC.Core.Security.Authentication.Membership;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class FubuPrincipalTester
    {
        [Fact]
        public void is_in_role_will_delegate()
        {
            var principal = new FubuPrincipal(new UserInfo{UserName = "somebody"}, role => role == "A");

            principal.IsInRole("A").ShouldBeTrue();
            principal.IsInRole("B").ShouldBeFalse();
        }

        [Fact]
        public void there_is_an_identity()
        {
            var principal = new FubuPrincipal(new UserInfo
            {
                UserName = "something"
            }, role => role == "A");

            principal.Identity.Name.ShouldBe("something");
        }

        [Fact]
        public void current_is_hooked_up()
        {
            var principal = new FubuPrincipal(new UserInfo
            {
                UserName = "something"
            }, role => role == "A");

            Thread.CurrentPrincipal = principal;

            FubuPrincipal.Current.ShouldBeTheSameAs(principal);
        }

        [Fact]
        public void set_current()
        {
            FubuPrincipal.SetCurrent(user => {
                user.UserName = "WreckItRalph";
                user.AddRoles("A", "B");
            });

            FubuPrincipal.Current.Identity.Name.ShouldBe("WreckItRalph");

            FubuPrincipal.Current.IsInRole("A").ShouldBeTrue();
            FubuPrincipal.Current.IsInRole("B").ShouldBeTrue();
            FubuPrincipal.Current.IsInRole("C").ShouldBeFalse();
        }
    }
}