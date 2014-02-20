using System;
using System.Reflection;
using Bottles.PackageLoaders.Assemblies;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
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

            graph.Add(action, ConfigurationType.Explicit);

            graph.ActionsFor(ConfigurationType.Explicit).Single()
                .ShouldBeTheSameAs(action);
        }

        [Test]
        public void add_configuration_action_that_is_marked_with_attribute()
        {
            var graph = new ConfigGraph();

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


            // Changed the access overrides to settings
            graph.ActionsFor(ConfigurationType.Settings).Any().ShouldBeTrue();

        }

        [Test]
        public void add_configuration_pack_has_to_be_idempotent()
        {
            var pack = new DiscoveryActionsConfigurationPack();
            var graph = new ConfigGraph();

            graph.Add(pack);

            var count = graph.ActionsFor(ConfigurationType.Discovery).Count();

            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());
            graph.Add(new DiscoveryActionsConfigurationPack());


            graph.ActionsFor(ConfigurationType.Discovery).Count()
                .ShouldEqual(count);
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