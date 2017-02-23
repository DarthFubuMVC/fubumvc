using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class AuthorizationRightTester
    {
        [Fact]
        public void combine_allow_beats_none()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.Allow);
        }

        [Fact]
        public void deny_trumps_all()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny)
                .ShouldBe(AuthorizationRight.Deny);

            AuthorizationRight.CombineRights(AuthorizationRight.Allow, AuthorizationRight.Deny, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.Deny);
        }

        [Fact]
        public void none_only_is_just_none()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None, AuthorizationRight.None)
                .ShouldBe(AuthorizationRight.None);
        }

        [Fact]
        public void one_value_is_just_that_value()
        {
            AuthorizationRight.CombineRights(AuthorizationRight.None).ShouldBe(AuthorizationRight.None);
            AuthorizationRight.CombineRights(AuthorizationRight.Allow).ShouldBe(AuthorizationRight.Allow);
            AuthorizationRight.CombineRights(AuthorizationRight.Deny).ShouldBe(AuthorizationRight.Deny);
        }

        [Fact]
        public void empty_permissions_is_equivalent_to_none()
        {
            AuthorizationRight.CombineRights().ShouldBe(AuthorizationRight.None);
        }
    }
}