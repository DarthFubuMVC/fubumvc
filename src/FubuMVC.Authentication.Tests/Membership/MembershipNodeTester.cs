using System;
using FubuMVC.Authentication.Membership;
using FubuMVC.Authentication.Membership.FlatFile;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Authentication.Tests.Membership
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
        public void can_build_the_object_def()
        {
            var node = MembershipNode.For<FlatFileMembershipRepository>();

            var def = node.As<IContainerModel>().ToObjectDef();

            def.DependencyFor<IMembershipRepository>()
               .As<ConfiguredDependency>()
               .Definition.Type.ShouldEqual(typeof (FlatFileMembershipRepository));
        }
    }
}