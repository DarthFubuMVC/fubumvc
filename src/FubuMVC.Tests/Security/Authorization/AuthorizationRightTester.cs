using FubuMVC.Core.Security.Authorization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class AuthorizationRightTester
    {
        [Test]
        public void combine_allow_beats_none()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.Allow);
        }

        [Test]
        public void deny_trumps_all()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny)
                .ShouldBe(AuthorizationRight.Deny);

            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void none_only_is_just_none()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.None);
        }

        [Test]
        public void one_value_is_just_that_value()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None).ShouldBe(AuthorizationRight.None);
            AuthorizationRight.CombineRights(AuthorizationRight.Allow).ShouldBe(AuthorizationRight.Allow);
            AuthorizationRight.CombineRights(AuthorizationRight.Deny).ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void empty_permissions_is_equivalent_to_none()
        {
            AuthorizationRight.CombineRights().ShouldBe(AuthorizationRight.None);
        }
    }
}