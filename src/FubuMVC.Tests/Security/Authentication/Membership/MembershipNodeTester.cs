using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.StructureMap;
using Shouldly;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Security.Authentication.Membership
{
    [TestFixture]
    public class MembershipNodeTester
    {
        [Test]
        public void can_only_take_in_a_membership_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new MembershipNode(GetType());
            });
        }

        [Test]
        public void can_build_the_instance_configuration()
        {
            var node = MembershipNode.For<FakeMembershipRepository>();

            var def = node.As<IContainerModel>().ToInstance().As<IConfiguredInstance>();

            def.FindDependencyDefinitionFor<IMembershipRepository>()
                .ReturnedType.ShouldBe(typeof(FakeMembershipRepository));
        }
    }

    public class FakeMembershipRepository : IMembershipRepository
    {
        public bool MatchesCredentials(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public IUserInfo FindByName(string username)
        {
            throw new NotImplementedException();
        }
    }
}