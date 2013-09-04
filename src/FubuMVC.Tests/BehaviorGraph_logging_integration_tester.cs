using System.Diagnostics;
using AssemblyPackage;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using System.Linq;
using System.Collections.Generic;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class BehaviorGraph_logging_integration_tester
    {
        private BehaviorGraph theGraph;
        private Container container;
        private LoggedFubuRegistry theRegistry;
        private ConfigLog theLogs;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new LoggedFubuRegistry();

            // Do it this way so that it gets the assembly package
            container = new Container();
            theGraph = FubuApplication.For(theRegistry).StructureMap(container)
                .Bootstrap().Factory.Get<BehaviorGraph>();

            theGraph.BehaviorFor<LoggedEndpoint>(x => x.get_logged_hello()).ShouldNotBeNull();

            theLogs = container.GetInstance<ConfigLog>();
        }

        [Test]
        public void the_container_has_the_config_log_with_the_config_graph_from_this_application()
        {
            var log = container.GetInstance<ConfigLog>();
            var chains = log.UniqueProvenanceChains();

            chains.Any().ShouldBeTrue();

            chains.Any(x => x.Has(new FubuRegistryProvenance(theRegistry))).ShouldBeTrue();
        }

        [Test]
        public void provenance_chain_from_a_bottle()
        {
            ITracedModel model = theGraph.BehaviorFor<AssemblyEndpoint>(x => x.get_hello()).As<ITracedModel>();
            var chain = model.AllEvents().OfType<Created>().Single().Source.ProvenanceChain.Chain;

            chain.Each(x => Debug.WriteLine(x));

            var assemblyPackage = PackageRegistry.Packages.Single(x => x.Name == typeof (AssemblyEndpoint).Assembly.GetName().Name);
            assemblyPackage.ShouldNotBeNull();

            chain.ShouldContain(new BottleProvenance(assemblyPackage));
            chain.OfType<FubuRegistryProvenance>().Last().Registry.ShouldBeOfType<AssemblyPackageRegistry>();
            
        }

        [Test]
        public void provenance_chain_from_an_extension_loaded_from_a_bottle()
        {
            var chain = theGraph.BehaviorFor<AssemblyEndpoint>(x => x.get_hello());
            var wrapper = chain.OfType<Wrapper>().Single(x => x.BehaviorType == typeof (BehaviorFromAssemblyBottle));

            var provenance = wrapper.As<ITracedModel>().AllEvents().OfType<Created>().Single()
                .Source.ProvenanceChain;


            provenance.OfType<FubuRegistryExtensionProvenance>().Single()
                .Extension.ShouldBeOfType<AssemblyPackageExtension>();
        }

        [Test]
        public void provenance_chain_from_a_fubu_registry_extension()
        {
            ITracedModel model = theGraph.BehaviorFor<RandomExtension>(x => x.SayHello()).As<ITracedModel>();
            var chain = model.AllEvents().OfType<Created>().Single().Source.ProvenanceChain.Chain;
            chain.ElementAt(0).ShouldEqual(new FubuRegistryProvenance(theRegistry));
            chain.ElementAt(1).ShouldBeOfType<FubuRegistryExtensionProvenance>().Extension
                .ShouldBeOfType<RandomExtension>();

        }

        [Test]
        public void captures_provenance_on_writers()
        {
            var events = theLogs.EventsOfType<Created>().Where(x => x.Subject is WriterNode);
            events.Any().ShouldBeTrue();

            events.Each(x => {
                x.Source.ProvenanceChain.ShouldNotBeNull();
            });
        }

        [Test]
        public void captures_provenance_on_readers()
        {
            var events = theLogs.EventsOfType<Created>().Where(x => x.Subject is ReaderNode);
            events.Any().ShouldBeTrue();

            events.Each(x =>
            {
                x.Source.ProvenanceChain.ShouldNotBeNull();
            });
        }

        [Test]
        public void provenance_chain_from_a_configuration_pack()
        {
            var chain = theGraph.BehaviorFor<AssemblyEndpoint>(x => x.get_hello());
            chain.Output.Writers.First().As<ITracedModel>().AllEvents().Each(x => {
                x.Source.ProvenanceChain.Single().ShouldBeOfType<ConfigurationPackProvenance>().Pack.ShouldBeOfType
                    <DefaultConfigurationPack>();
            });
        }

        [Test]
        public void every_service_added_has_a_provenance()
        {
            var events = theLogs.EventsOfType<ServiceEvent>().ToList();
            events.Any().ShouldBeTrue();
            events.Each(x => {
                Debug.WriteLine(x.Source.ProvenanceChain);
                x.Source.ProvenanceChain.ShouldNotBeNull();
            });
        }
    }

    public class LoggedFubuRegistry : FubuRegistry
    {
        public LoggedFubuRegistry()
        {
            Import<RandomExtension>();
        }
    }

    public class RandomExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Route("hello").Calls<RandomExtension>(x => x.SayHello());
        }

        public string SayHello()
        {
            return "Hello";
        }
    }

    public class OddballController
    {
        public string get_goodbye()
        {
            return "goodbye";
        }
    }

    public class LoggedEndpoint
    {
        public string get_logged_hello()
        {
            return "hello";
        }
    }
}