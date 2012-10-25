using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;
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
        public void the_ConfigurationGraph_is_registered_in_the_services_for_later()
        {
            theGraph.Services.ServicesFor(typeof (ConfigGraph)).Single().Value.ShouldNotBeNull();
        }

        [Test]
        public void source_and_chain_are_associated_with_each_event()
        {
            theGraph.Behaviors.OfType<ITracedModel>().Any().ShouldBeTrue();

            theGraph.Behaviors.OfType<ITracedModel>().Each(chain =>
            {
                chain.AllEvents().Each(e =>
                {
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
                });
            });
        }

        [Test]
        public void each_configuration_source_has_the_provenance_set_simple_case_of_only_one_FubuRegistry()
        {
            Assert.Fail("NWO");
//            var registry = new FubuRegistry();
//            registry.Actions.IncludeClassesSuffixedWithController();
//
//            var graph = BehaviorGraph.BuildFrom(registry);
//
//            graph.Log.AllConfigSources().Any().ShouldBeTrue();
//            graph.Log.AllConfigSources().Each(x => x.Provenance.ShouldBeTheSameAs(registry));
        }


        [Test]
        public void capture_the_configuration_source_from_imports()
        {
            Assert.Fail("NWO");
//            var registry = new FubuRegistry();
//            registry.Actions.IncludeClassesSuffixedWithController();
//            registry.Import<FakeRegistry>();
//
//            var graph = BehaviorGraph.BuildFrom(registry);
//
//            var chain = graph.BehaviorFor<FakeThing>(x => x.SayHello()).As<ITracedModel>();
//
//            // This is simple
//            chain.AllEvents().OfType<Created>().Single().Source.Provenance.ShouldBeOfType<FakeRegistry>();
//            chain.AllEvents().OfType<ChainImported>().Single().Source.Action.ShouldBeOfType<RegistryImport>();
//
//            graph.Log.AllConfigSources().Where(x => x.Provenance is FakeRegistry).Any().ShouldBeTrue();
//        


        }
    }


    public class FakeRegistry : FubuPackageRegistry
    {
        public FakeRegistry()
        {
            Actions.IncludeType<FakeThing>();
        }
    }

    public class FakeThing
    {
        public string SayHello()
        {
            return "Hello!";
        }
    }
}