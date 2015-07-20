using FubuMVC.Core.Security.Authorization;
using FubuTestingSupport;
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
                .ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void deny_trumps_all()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny)
                .ShouldEqual(AuthorizationRight.Deny);

            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny, AuthorizationRight.None)
                .ShouldEqual(AuthorizationRight.Deny);
        }

        [Test]
        public void none_only_is_just_none()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None, AuthorizationRight.None)
                .ShouldEqual(AuthorizationRight.None);
        }

        [Test]
        public void one_value_is_just_that_value()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None).ShouldEqual(AuthorizationRight.None);
            AuthorizationRight.CombineRights(AuthorizationRight.Allow).ShouldEqual(AuthorizationRight.Allow);
            AuthorizationRight.CombineRights(AuthorizationRight.Deny).ShouldEqual(AuthorizationRight.Deny);
        }

        [Test]
        public void empty_permissions_is_equivalent_to_none()
        {
            AuthorizationRight.CombineRights().ShouldEqual(AuthorizationRight.None);
        }
    }
}