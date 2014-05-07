using System;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class AuthorizedByAttributeConventionTester
    {
        private BehaviorGraph graph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<AuthorizedByAttributeConventionController>();

            graph = BehaviorGraph.BuildFrom(registry);
        }

        private BehaviorChain chainFor(Expression<Action<AuthorizedByAttributeConventionController>> action)
        {
            return graph.BehaviorFor(action);
        }

        [Test]
        public void get_policy_type_when_it_is_an_authorization_policy()
        {
            AuthorizedByAttribute.RuleTypeFor(typeof (Input1), typeof (AuthorizationRule1))
                .ShouldEqual(typeof (AuthorizationRule1));
        }


        [Test]
        public void no_authorization_rules_on_a_method_not_decorated_with_attributes()
        {
            chainFor(x => x.MethodWithNoAttributes()).Authorization.HasRules().ShouldBeFalse();
        }

        [Test]
        public void authorization_rule_for_a_single_policy_on_a_method()
        {
            chainFor(x => x.MethodWithOnePolicy()).Authorization.Policies.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AuthorizationRule1));
        }


        [Test]
        public void authorization_rules_for_multiple_policies_on_a_method()
        {
            chainFor(x => x.MethodWithMultiplePolicies()).Authorization.Policies.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AuthorizationRule1), typeof(AuthorizationRule2));
        }

    }

    [TestFixture]
    public class AuthorizedByAttributeConventionAtTheClassLevel
    {
        private BehaviorGraph graph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<AuthorizedController2>();

            graph = BehaviorGraph.BuildFrom(registry);
        }

        private BehaviorChain chainFor(Expression<Action<AuthorizedController2>> action)
        {
            return graph.BehaviorFor(action);
        }

        [Test]
        public void rules_should_be_picked_up_from_handler_type()
        {
            chainFor(x => x.MethodWithNoAttributes()).Authorization.Policies.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AuthorizationRule2));
        }

        [Test]
        public void rules_in_combination_of_method_and_handler_type()
        {
            var rules = chainFor(x => x.MethodWithOnePolicy()).Authorization.Policies.Select(x => x.GetType()).ToList();
            rules
                .ShouldHaveTheSameElementsAs(typeof(AuthorizationRule2), typeof(AuthorizationRule1));
        }
    }

    [AuthorizedBy(typeof(AuthorizationRule2))]
    public class AuthorizedController2
    {
        public void MethodWithNoAttributes() { }

        [AuthorizedBy(typeof(AuthorizationRule1))]
        public void MethodWithOnePolicy() { }

        public void MethodWithOneRule(Input1 input)
        {

        }
    }

    public class AuthorizedByAttributeConventionController
    {
        public void MethodWithNoAttributes() { }

        [AuthorizedBy(typeof(AuthorizationRule1))]
        public void MethodWithOnePolicy(){}

        [AuthorizedBy(typeof(AuthorizationRule1), typeof(AuthorizationRule2))]
        public void MethodWithMultiplePolicies() { }

        public void MethodWithOneRule(Input1 input)
        {
            
        }

        public void MethodWithMultipleRules(Input1 input)
        {

        }

        [AuthorizedBy(typeof(AuthorizationRule1), typeof(AuthorizationRule2))]
        public void MixedMethod(Input1 input)
        {
            
        }
    }

    public class Input1{}
    public class Input2{}



    public class AuthorizationRule1 : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            throw new NotImplementedException();
        }
    }

    public class AuthorizationRule2 : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequestContext request)
        {
            throw new NotImplementedException();
        }
    }

    public class NotAnAuthorizationRule{}


}