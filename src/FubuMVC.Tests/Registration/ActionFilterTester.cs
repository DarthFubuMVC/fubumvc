using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ActionFilterTester
    {
        private BehaviorGraph theGraph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom(r => {
                r.Actions.IncludeType<SomeEndpoint>();

                r.Configure(g => {
                    var chain = g.BehaviorFor<SomeEndpoint>(x => x.get_something(null));
                    chain.Prepend(ActionFilter.For<SomeEndpoint>(x => x.Filter(null)));
                });
            });
        }

        [Test]
        public void the_filter_does_not_count_toward_input_or_resource_type()
        {
            var chain = theGraph.BehaviorFor<SomeEndpoint>(x => x.get_something(null));

            chain.InputType().ShouldEqual(typeof (RealInput));
            chain.ResourceType().ShouldEqual(typeof (RealOutput));
        }

        [Test]
        public void still_get_the_continuation_director_behind_the_action_filter()
        {
            var chain = theGraph.BehaviorFor<SomeEndpoint>(x => x.get_something(null));
            chain.FirstOrDefault(x => x is ActionFilter).Next
                .ShouldBeOfType<ContinuationNode>();
        }

        [Test]
        public void does_not_impact_a_normal_action_call()
        {
            var chain = theGraph.BehaviorFor<SomeEndpoint>(x => x.get_somewhere(null));
            chain.ResourceType().ShouldEqual(typeof (FubuContinuation));
            chain.InputType().ShouldEqual(typeof (RealInput));
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