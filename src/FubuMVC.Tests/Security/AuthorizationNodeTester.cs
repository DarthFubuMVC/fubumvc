using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Urls;
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
            authorizationBehavior.Policies.Count().ShouldEqual(1);
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
        public void adding_multiple_roles()
        {
            var node = new AuthorizationNode();
            node.AddRole("RoleA");
            node.AddRole("RoleB");
            node.AddRole("RoleC");

            var authorizationBehavior = toBehavior(node);
            authorizationBehavior.Policies.Count().ShouldEqual(3);

            authorizationBehavior.Policies.ToArray()[0].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleA");
            authorizationBehavior.Policies.ToArray()[1].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleB");
            authorizationBehavior.Policies.ToArray()[2].ShouldBeOfType<AllowRole>().Role.ShouldEqual("RoleC");
        }


        [Test]
        public void use_no_custom_auth_failure_handler()
        {
            var node = new AuthorizationNode();
            var def = node.As<IContainerModel>().ToObjectDef();

            def.DependencyFor<IAuthorizationFailureHandler>().ShouldBeNull();
        }

        [Test]
        public void use_custom_auth_failure_handler_by_type()
        {
            var node = new AuthorizationNode();
            node.FailureHandler<FakeAuthHandler>();

            var def = node.As<IContainerModel>().ToObjectDef();

            def.DependencyFor<IAuthorizationFailureHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(typeof (FakeAuthHandler));
        }

        [Test]
        public void use_custom_failure_handler_by_value()
        {
            var node = new AuthorizationNode();

            var handler = new FakeAuthHandler();

            node.FailureHandler(handler);

            var def = node.As<IContainerModel>().ToObjectDef();

            def.DependencyFor<IAuthorizationFailureHandler>().ShouldBeOfType<ConfiguredDependency>()
                .Definition.Value.ShouldBeTheSameAs(handler);
        }

        [Test]
        public void add_type_for_a_policy()
        {
            var node = new AuthorizationNode();
            node.Add(typeof(AlwaysAllowPolicy));

            node.Policies.Single().ShouldBeOfType<AlwaysAllowPolicy>();
        }

        [Test]
        public void add_type_for_check()
        {
            var node = new AuthorizationNode();
            node.Add(typeof(FakeAuthCheck));

            node.Policies.Single().ShouldBeOfType<AuthorizationCheckPolicy<FakeAuthCheck>>();
        }

        [Test]
        public void invalid_add_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new AuthorizationNode().Add(GetType());
            });
        }

        [Test]
        public void invalid_add_type_if_policy_type_has_args()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new AuthorizationNode().Add(typeof(PolicyWithArgs));
            });
        }
    }

    public class PolicyWithArgs : IAuthorizationPolicy
    {
        public PolicyWithArgs(int number)
        {
        }

        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAuthCheck : IAuthorizationCheck
    {
        public AuthorizationRight Check()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAuthHandler : IAuthorizationFailureHandler
    {
        public FubuContinuation Handle()
        {
            throw new NotImplementedException();
        }
    }
}