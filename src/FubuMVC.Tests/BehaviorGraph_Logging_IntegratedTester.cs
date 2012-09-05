using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Collections.Generic;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class BehaviorGraph_Logging_IntegratedTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();
            });
        }

        [Test]
        public void source_and_chain_are_associated_with_each_event()
        {
            theGraph.Behaviors.OfType<ITracedModel>().Any().ShouldBeTrue();

            theGraph.Behaviors.OfType<ITracedModel>().Each(chain =>
            {
                chain.AllEvents().Each(e =>
                {
                    e.Chain.ShouldBeTheSameAs(chain);
                    e.Source.ShouldNotBeNull();
                    e.Subject.ShouldBeTheSameAs(chain);
                });
            });
        }

        [Test]
        public void source_and_chain_are_associated_with_each_node_event_on_each_node()
        {
            var chain = theGraph.Behaviors.First();

            chain.OfType<ITracedModel>().Each(node =>
            {
                node.AllEvents().Any().ShouldBeTrue();
                node.AllEvents().Each(e =>
                {
                    e.Subject.ShouldBeTheSameAs(node);
                    e.Source.ShouldNotBeNull();
                    e.Chain.ShouldBeTheSameAs(chain);
                });
            });
        }
    }
}