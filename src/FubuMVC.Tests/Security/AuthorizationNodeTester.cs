using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class AuthorizationNodeTester
    {
        [Test]
        public void category_is_authorization()
        {
            new AuthorizationNode().Category.ShouldEqual(BehaviorCategory.Authorization);
        }

        [Test]
        public void no_rules()
        {
            new AuthorizationNode().HasRules().ShouldBeFalse();
        }

        [Test]
        public void with_rules()
        {
            var node = new AuthorizationNode();
            node.AddRole("Role A");

            node.HasRules().ShouldBeTrue();
        }

        private AuthorizationBehavior toBehavior(AuthorizationNode node)
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            return container.GetInstance<AuthorizationBehavior>(new ObjectDefInstance(node.ToObjectDef()));
        }

        [Test]
        public void adding_a_role()
        {
            var node = new AuthorizationNode();
            node.AddRole("RoleA");

            var authorizationBehavior = toBehavior(node);
            authorizationBehavior.Policies.Count.ShouldEqual(1);
            authorizationBehavior.Policies.First().ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleA");
        }

        [Test]
        public void adding_multiple_roles()
        {
            var node = new AuthorizationNode();
            node.AddRole("RoleA");
            node.AddRole("RoleB");
            node.AddRole("RoleC");

            var authorizationBehavior = toBehavior(node);
            authorizationBehavior.Policies.Count.ShouldEqual(3);

            authorizationBehavior.Policies[0].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleA");
            authorizationBehavior.Policies[1].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleB");
            authorizationBehavior.Policies[2].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleC");
        }
    }
}