using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using Rhino.Mocks;
using StructureMap;

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
            return container.GetInstance<AuthorizationBehavior>(new ObjectDefInstance(node.As<IContainerModel>().ToObjectDef()));
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
        public void adding_a_policy()
        {
            var node = new AuthorizationNode();
            var policy = MockRepository.GenerateMock<IAuthorizationPolicy>();

            node.AddPolicy(policy);

            var authorizationBehavior = toBehavior(node);
            authorizationBehavior.Policies.Single().ShouldBeTheSameAs(policy);
        }



        [Test]
        public void adding_a_model_rule_policy()
        {
            var node = new AuthorizationNode();
            node.AddPolicy<UrlModel, UrlModelShouldStartWithJ>();

            toBehavior(node).Policies.Single().ShouldBeOfType<AuthorizationPolicy<UrlModel>>()
                .InnerRule.ShouldBeOfType<UrlModelShouldStartWithJ>();
        }

        [Test]
        public void adding_a_rule()
        {
            var node = new AuthorizationNode();
            node.AddRule(typeof (UrlModelShouldStartWithJ));

            toBehavior(node).Policies.Single().ShouldBeOfType<AuthorizationPolicy<UrlModel>>()
                .InnerRule.ShouldBeOfType<UrlModelShouldStartWithJ>();
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

    [TestFixture]
    public class when_creating_an_object_def_for_an_endpoint_authorizor
    {
        private ObjectDef endpointObjectDef;
        private BehaviorChain chain;

        [SetUp]
        public void SetUp()
        {
            chain = new BehaviorChain();

            var node = new AuthorizationNode();
            node.AddRole("RoleA");
            node.AddRole("RoleB");
            node.AddRole("RoleC");

            chain.AddToEnd(node);

            endpointObjectDef = node.As<IAuthorizationRegistration>().ToEndpointAuthorizorObjectDef();
        }

        [Test]
        public void the_type_should_be_endpoint_authorizor()
        {
            endpointObjectDef.Type.ShouldEqual(typeof (EndPointAuthorizor));
        }

        [Test]
        public void the_name_should_be_the_behavior_id_from_the_parent_chain()
        {
            endpointObjectDef.Name.ShouldEqual(chain.Top.As<IContainerModel>().ToObjectDef().Name);
        }

        [Test]
        public void should_have_a_list_dependency_for_the_authorization_policies()
        {
            var dependency = endpointObjectDef.Dependencies.Single().ShouldBeOfType<ListDependency>();
            dependency.Items.Select(x => x.Value.ShouldBeOfType<AllowRole>().Role)
                .ShouldHaveTheSameElementsAs("RoleA", "RoleB", "RoleC");


        
        }

        [Test]
        public void make_sure_we_can_actually_build_it()
        {
            var instance = new ObjectDefInstance(endpointObjectDef);
            var container = new Container();

            container.GetInstance<IEndPointAuthorizor>(instance)
                .ShouldBeOfType<EndPointAuthorizor>()
                .Policies.Cast<AllowRole>().Select(x => x.Role)
                .ShouldHaveTheSameElementsAs("RoleA", "RoleB", "RoleC");
        }
    }
}