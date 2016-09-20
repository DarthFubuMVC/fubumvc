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


    }
}