using System.Diagnostics;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class WrapWithAttributeConventionTester
    {
        private BehaviorGraph graph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<WrapWithAttributeController>();

            graph = BehaviorGraph.BuildFrom(registry);

            graph.Behaviors.Count().ShouldBeGreaterThan(0);

        }

        [Test]
        public void should_have_a_call_in_each_behavior_chain()
        {
            int callCount = graph.Behaviors.SelectMany(x => x.Calls).Count();
            callCount.ShouldEqual(graph.Behaviors.Count());
        }

        [Test]
        public void place_no_wrappers_on_actions_that_do_not_have_the_attribute()
        {
            graph.BehaviorFor<WrapWithAttributeController>(x => x.MethodWithNoAttributes()).First()
                .ShouldNotBeOfType<Wrapper>();
        }

        [Test]
        public void place_wrapper_on_action_with_a_single_attribute()
        {
            var chain = graph.BehaviorFor<WrapWithAttributeController>(x => x.MethodWithOneAttribute());
            chain.First()
                .ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(typeof(Wrapper1));
        }

        [Test]
        public void place_wrapper_on_action_with_multiple_attributes()
        {
            var behaviors =
                graph.BehaviorFor<WrapWithAttributeController>(x => x.MethodWithMultipleAttributes()).ToList();


            behaviors[0].ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(typeof (Wrapper1));
            behaviors[1].ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(typeof (Wrapper2));
            behaviors[2].ShouldBeOfType<Wrapper>().BehaviorType.ShouldEqual(typeof (Wrapper3));
        }
    }

    public class WrapWithAttributeController
    {
        [WrapWith(typeof(Wrapper1))]
        public void MethodWithOneAttribute() { }

        [WrapWith(typeof(Wrapper1), typeof(Wrapper2), typeof(Wrapper3))]
        public void MethodWithMultipleAttributes(){}
        public void M3() { }
        public void M4() { }
        public void M5() { }
        public void MethodWithNoAttributes() { }
        
        
    }
}