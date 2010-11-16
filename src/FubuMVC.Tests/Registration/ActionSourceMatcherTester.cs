using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ActionSourceMatcherTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new BehaviorGraph(null);
            matcher = new ActionSourceMatcher();

            pool = new TypePool(null);
        }

        #endregion

        private BehaviorGraph graph;
        private ActionSourceMatcher matcher;
        private TypePool pool;
        
        [Test]
        public void chains_are_registered_from_source()
        {
            matcher.AddSource(new SimpleActionSource());
            matcher.BuildBehaviors(pool, graph);

            graph.Behaviors.ShouldHaveCount(1);
        }
    }

    public class SimpleActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            yield return ActionCall.For<SimpleController>(c => c.Action());
        }
    }

    public class SimpleController
    {
        public void Action()
        {
        }
    }
}