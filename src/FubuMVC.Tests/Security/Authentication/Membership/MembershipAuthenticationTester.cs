﻿using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Membership
{
    
    public class MembershipAuthenticationTester : InteractionContext<MembershipAuthentication>
    {
        [Fact]
        public void authentication_is_a_straight_up_delegation_positive()
        {
            var request = new LoginRequest
            {
                UserName = "foo",
                Password = "bar"
            };
            MockFor<IMembershipRepository>().Stub(x => x.MatchesCredentials(request))
                                            .Return(true);


            ClassUnderTest.AuthenticateCredentials(request).ShouldBeTrue();
        }

        [Fact]
        public void empty_user_name_is_automatically_negative()
        {
            ClassUnderTest.AuthenticateCredentials(new LoginRequest
            {
                UserName = null,
                Password = "something"
            }).ShouldBeFalse();

            ClassUnderTest.AuthenticateCredentials(new LoginRequest
            {
                UserName = "something",
                Password = null
            }).ShouldBeFalse();
        }

        [Fact]
        public void authentication_is_a_straight_up_delegation_negative()
        {
            var request = new LoginRequest
            {
                UserName = "foo",
                Password = "bar"
            };
            MockFor<IMembershipRepository>().Stub(x => x.MatchesCredentials(request))
                                            .Return(false);


            ClassUnderTest.AuthenticateCredentials(request).ShouldBeFalse();
        }

        [Fact]
        public void build_principal()
        {
            var user = new UserInfo
            {
                UserName = "ralph"

            };

            var model = new AuthenticatedModel();
            user.Set(model);

            MockFor<IMembershipRepository>().Stub(x => x.FindByName(user.UserName))
                                            .Return(user);

            var principal = ClassUnderTest.Build(user.UserName);

            var fubuPrincipal = principal.ShouldBeOfType<FubuPrincipal>();
            fubuPrincipal.Identity.Name.ShouldBe(user.UserName);

            fubuPrincipal.Get<AuthenticatedModel>().ShouldBeTheSameAs(model);
        }
    }
}