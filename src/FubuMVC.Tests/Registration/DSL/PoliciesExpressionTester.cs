using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.Registration.Conventions;
using Shouldly;
using Xunit;
using System.Linq;

namespace FubuMVC.Tests.Registration.DSL
{
    
    public class when_applying_policies_for_wrappers_and_ordering
    {
        private BehaviorGraph graph;

        public when_applying_policies_for_wrappers_and_ordering()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OrderingPolicyController>();

            registry.Policies.Local.Configure(g => g.WrapAllWith<OPWrapper1>());

            registry.Policies.Global.Reorder(policy => {
                policy.ThisWrapperBeBefore<OPWrapper1>();
                policy.CategoryMustBeAfter = BehaviorCategory.Authorization;
            });

            graph = BehaviorGraph.BuildFrom(registry);
        }

        
        [Fact]
        public void move_behavior_before_authorization()
        {
            // Ordinarily, AuthorizationNode would be before any other behavior wrappers

            var chain = graph.ChainFor<OrderingPolicyController>(x => x.M1());
            chain.First().ShouldBeOfType<Wrapper>().BehaviorType.ShouldBe(typeof(OPWrapper1));
            chain.ToList()[1].ShouldBeOfType<AuthorizationNode>();
        }
    }

    
    public class when_defining_a_new_reordering_rule_inline
    {
        private BehaviorGraph graph;

        public when_defining_a_new_reordering_rule_inline()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OrderingPolicyController>();

            registry.Policies.Local.Configure(g => g.WrapAllWith<OPWrapper1>());

            registry.Policies.Global.Reorder(x =>
            {
                x.ThisWrapperBeBefore<OPWrapper1>();
                x.ThisNodeMustBeAfter<AuthorizationNode>();
            });

            graph = BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void move_behavior_before_authorization()
        {
            // Ordinarily, AuthorizationNode would be before any other behavior wrappers

            var chain = graph.ChainFor<OrderingPolicyController>(x => x.M1());
            chain.First().ShouldBeOfType<Wrapper>().BehaviorType.ShouldBe(typeof(OPWrapper1));
            chain.ToList()[1].ShouldBeOfType<AuthorizationNode>();
        }
    }

    
    public class when_adding_an_iconfigurationaction_without_specifying_configuration_type
    {
        [Fact]
        public void it_should_default_to_policy()
        {
            FakePolicy.Count = 0;
            BehaviorGraph.BuildFrom(graph => 
                graph.Policies.Local.Add(new FakePolicy()));

            FakePolicy.Count.ShouldBe(1);
        }
    }

    
    public class when_adding_an_action_of_policy_without_specifying_configuration_type
    {
        [Fact]
        public void it_should_default_to_policy()
        {
            FakePolicy.Count = 0;
            BehaviorGraph.BuildFrom(graph => 
                graph.Policies.Local.Add<FakePolicy>(x => x.NoOp()));

            FakePolicy.Count.ShouldBe(1);
        }
    }

    public class OrderingPolicyController
    {
        [WrapWith(typeof(OPWrapper2), typeof(OPWrapper3))]
        [AllowRole("R1")]
        public void M1(){}
        public void M2(){}
        public void M3(){}
        public void M4(){}
        public void M5(){}
    }

    public class OPWrapper1 : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    public class OPWrapper2 : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    public class OPWrapper3 : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    public class FakePolicy : IConfigurationAction
    {
        public static int Count;

        public void Configure(BehaviorGraph graph)
        {
            Count++;
        }

        public void NoOp() { }
    }
}