using System.Threading;
using FubuMVC.Core.Security.Authentication.Membership;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class FubuPrincipalTester
    {
        [Test]
        public void is_in_role_will_delegate()
        {
            var principal = new FubuPrincipal(new UserInfo{UserName = "somebody"}, role => role == "A");

            principal.IsInRole("A").ShouldBeTrue();
            principal.IsInRole("B").ShouldBeFalse();
        }

        [Test]
        public void there_is_an_identity()
        {
            var principal = new FubuPrincipal(new UserInfo
            {
                UserName = "something"
            }, role => role == "A");

            principal.Identity.Name.ShouldBe("something");
        }

        [Test]
        public void current_is_hooked_up()
        {
            var principal = new FubuPrincipal(new UserInfo
            {
                UserName = "something"
            }, role => role == "A");

            Thread.CurrentPrincipal = principal;

            FubuPrincipal.Current.ShouldBeTheSameAs(principal);
        }

        [Test]
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