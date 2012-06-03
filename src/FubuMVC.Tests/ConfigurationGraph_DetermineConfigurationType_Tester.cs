using System;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.UI.Navigation;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConfigurationGraph_DetermineConfigurationType_Tester
    {
        [ConfigurationType(ConfigurationType.Policy)]
        public class FakePolicy1 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }
        }

        [ConfigurationType(ConfigurationType.Explicit)]
        public class FakePolicy2 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }
        }

        [Policy]
        public class FakePolicy3 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }
        }


        [Discovery]
        public class FakePolicy4 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void reads_from_attribute()
        {
            ConfigurationGraph.DetermineConfigurationType(new FakePolicy1())
                .ShouldEqual(ConfigurationType.Policy);


            ConfigurationGraph.DetermineConfigurationType(new FakePolicy2())
                .ShouldEqual(ConfigurationType.Explicit);


            ConfigurationGraph.DetermineConfigurationType(new FakePolicy3())
                .ShouldEqual(ConfigurationType.Policy);

            ConfigurationGraph.DetermineConfigurationType(new FakePolicy4())
                .ShouldEqual(ConfigurationType.Discovery);
        }

        [Test]
        public void navigation_registry_is_navigation_duh()
        {
            ConfigurationGraph.DetermineConfigurationType(new NavigationRegistry())
                .ShouldEqual(ConfigurationType.Navigation);
        }

        [Test]
        public void reorder_policy_is_reording()
        {
            ConfigurationGraph.DetermineConfigurationType(new ReorderBehaviorsPolicy())
                .ShouldEqual(ConfigurationType.Reordering);
        }

        [Test]
        public void ServiceRegistry_is_services()
        {
            ConfigurationGraph.DetermineConfigurationType(new ServiceRegistry())
                .ShouldEqual(ConfigurationType.Services);
        }

        [Test]
        public void lambda_should_be_indeterminate()
        {
            ConfigurationGraph.DetermineConfigurationType(new LambdaConfigurationAction(g => { }))
                .ShouldEqual(null);
        }


    }
}