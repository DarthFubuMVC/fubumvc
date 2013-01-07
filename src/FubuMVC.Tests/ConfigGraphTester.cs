using System;
using System.Reflection;
using Bottles.PackageLoaders.Assemblies;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;
using Rhino.Mocks;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConfigGraphTester
    {
        [Test]
        public void push_bottle()
        {
            var bottle = new AssemblyPackageInfo(Assembly.GetExecutingAssembly());
            var graph = new ConfigGraph();

            graph.Push(bottle);

            graph.CurrentProvenance.ShouldHaveTheSameElementsAs(new BottleProvenance(bottle));


        }

        [Test]
        public void push_extension()
        {
            var bottle = new AssemblyPackageInfo(Assembly.GetExecutingAssembly());
            var graph = new ConfigGraph();

            graph.Push(bottle);
            var extension = new FakeRegistryExtension();
            graph.Push(extension);

            graph.CurrentProvenance.ShouldHaveTheSameElementsAs(new BottleProvenance(bottle), new FubuRegistryExtensionProvenance(extension));
        }

        [Test]
        public void Pop()
        {
            var bottle = new AssemblyPackageInfo(Assembly.GetExecutingAssembly());
            var graph = new ConfigGraph();

            graph.Push(bottle);
            var extension = new FakeRegistryExtension();
            graph.Push(extension);

            graph.Pop();

            graph.CurrentProvenance.ShouldHaveTheSameElementsAs(new BottleProvenance(bottle));

            graph.Pop();

            graph.CurrentProvenance.Any().ShouldBeFalse();
        }




        [Test]
        public void push_fubu_registry()
        {
            var registry = new SomeFubuRegistry(); 
            var graph = new ConfigGraph();
            graph.Push(registry);

            graph.CurrentProvenance.Single().ShouldEqual(new FubuRegistryProvenance(registry));
        }

        [Test]
        public void fill_action_puts_the_provenance_in_the_right_order()
        {
            var bottle = new AssemblyPackageInfo(Assembly.GetExecutingAssembly());
            var graph = new ConfigGraph();

            graph.Push(bottle);
            var extension = new FakeRegistryExtension();
            graph.Push(extension);

            var policy = new UniquePolicy();
            graph.Add(policy, ConfigurationType.Policy);



            graph.LogsFor(ConfigurationType.Policy).Single()
                .ProvenanceChain.ShouldHaveTheSameElementsAs(new BottleProvenance(bottle), new FubuRegistryExtensionProvenance(extension));
        }

        [Test]
        public void add_configuration_action_with_indeterminate_ConfigurationType()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new ConfigGraph().Add(new IndeterminateAction(), null);
            });
        }

        [Test]
        public void add_configuration_action_with_default_configuration_type()
        {
            var graph = new ConfigGraph();

            var action = new IndeterminateAction();

            graph.Push(new SomeFubuRegistry());
            graph.Add(action, ConfigurationType.Explicit);

            graph.ActionsFor(ConfigurationType.Explicit).Single()
                .ShouldBeTheSameAs(action);
        }

        [Test]
        public void add_configuration_action_that_is_marked_with_attribute()
        {
            var graph = new ConfigGraph();
            graph.Push(new SomeFubuRegistry());

            var action = new DeterminateAciton();

            graph.Add(action);

            graph.ActionsFor(ConfigurationType.Conneg).Single()
                .ShouldBeTheSameAs(action);
        }

        [Test]
        public void add_configuration_pak()
        {
            var pack = new DiscoveryActionsConfigurationPack();
            var graph = new ConfigGraph();

            graph.Add(pack);

            graph.LogsFor(ConfigurationType.Discovery).Each(log => {
                log.ProvenanceChain.ShouldHaveTheSameElementsAs(new ConfigurationPackProvenance(pack));
            });

            // Changed the access overrides to settings
            graph.LogsFor(ConfigurationType.Settings).Any().ShouldBeTrue();

        }

        [Test]
        public void add_configuration_pack_has_to_be_idempotent()
        {
            var pack = new DiscoveryActionsConfigurationPack();
            var graph = new ConfigGraph();

            graph.Add(pack);

            var count = graph.LogsFor(ConfigurationType.Discovery).Count();

            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());


            graph.LogsFor(ConfigurationType.Discovery).Count()
                .ShouldEqual(count);
        }

        [Test]
        public void prepend_provenance()
        {
            var graph = new ConfigGraph();
            var defaultConfigurationPack = new DefaultConfigurationPack();
            graph.Add(defaultConfigurationPack);

            var p1 = MockRepository.GenerateMock<Provenance>();
            var p2 = MockRepository.GenerateMock<Provenance>();

            graph.PrependProvenance(new Provenance[]{p1, p2});

            graph.LogsFor(ConfigurationType.Conneg).Each(log => {
                log.ProvenanceChain.ShouldHaveTheSameElementsAs(p1, p2, new ConfigurationPackProvenance(defaultConfigurationPack));
            });

            graph.LogsFor(ConfigurationType.ModifyRoutes).Each(log =>
            {
                log.ProvenanceChain.ShouldHaveTheSameElementsAs(p1, p2, new ConfigurationPackProvenance(defaultConfigurationPack));
            });

            graph.LogsFor(ConfigurationType.InjectNodes).Each(log =>
            {
                log.ProvenanceChain.ShouldHaveTheSameElementsAs(p1, p2, new ConfigurationPackProvenance(defaultConfigurationPack));
            });

            graph.LogsFor(ConfigurationType.Attachment).Each(log =>
            {
                log.ProvenanceChain.ShouldHaveTheSameElementsAs(p1, p2, new ConfigurationPackProvenance(defaultConfigurationPack));
            });
        }
    }
    
    [ConfigurationType(ConfigurationType.Conneg)]
    public class DeterminateAciton : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public class IndeterminateAction : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public class SomeFubuRegistry : FubuRegistry
    {
        
    }

    public class FakeRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            throw new NotImplementedException();
        }
    }
}