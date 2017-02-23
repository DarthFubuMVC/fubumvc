﻿using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using Xunit;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    
    public class ActionFilterTester
    {
        private BehaviorGraph theGraph;

        public ActionFilterTester()
        {
            theGraph = BehaviorGraph.BuildFrom(r => {
                r.Actions.IncludeType<SomeEndpoint>();

                r.Configure(g => {
                    var chain = g.ChainFor<SomeEndpoint>(x => x.get_something(null));
                    chain.Prepend(ActionFilter.For<SomeEndpoint>(x => x.Filter(null)));
                });
            });
        }


        [Fact]
        public void the_filter_does_not_count_toward_input_or_resource_type()
        {
            var chain = theGraph.ChainFor<SomeEndpoint>(x => x.get_something(null));

            chain.InputType().ShouldBe(typeof (RealInput));
            chain.ResourceType().ShouldBe(typeof (RealOutput));
        }

        [Fact]
        public void still_get_the_continuation_director_behind_the_action_filter()
        {
            var chain = theGraph.ChainFor<SomeEndpoint>(x => x.get_something(null));
            chain.FirstOrDefault(x => x is ActionFilter).Next
                .ShouldBeOfType<ContinuationNode>();
        }

        [Fact]
        public void does_not_impact_a_normal_action_call()
        {
            var chain = theGraph.ChainFor<SomeEndpoint>(x => x.get_somewhere(null));
            chain.ResourceType().ShouldBe(typeof (FubuContinuation));
            chain.InputType().ShouldBe(typeof (RealInput));
        }
    }

    public class SomeEndpoint
    {
        public RealOutput get_something(RealInput input)
        {
            return null;
        }

        public FubuContinuation get_somewhere(RealInput input)
        {
            return null;
        }

        public FubuContinuation Filter(FilterInput input)
        {
            return FubuContinuation.NextBehavior();
        }
    }

    public class FilterInput{}
    public class RealInput{}
    public class RealOutput{}
}