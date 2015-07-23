using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Security.Authorization;
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
}