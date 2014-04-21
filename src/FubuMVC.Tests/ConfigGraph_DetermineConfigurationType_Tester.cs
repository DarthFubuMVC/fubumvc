using System;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConfigGraph_DetermineConfigurationType_Tester
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


        [ConfigurationType(ConfigurationType.Explicit)]
        public class FakePolicy4 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                throw new NotImplementedException();
            }
        }

        public class FakePolicy5 : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                
            }
        }

        [Test]
        public void reads_from_attribute()
        {
            ConfigGraph.DetermineConfigurationType(new FakePolicy1())
                .ShouldEqual(ConfigurationType.Policy);


            ConfigGraph.DetermineConfigurationType(new FakePolicy2())
                .ShouldEqual(ConfigurationType.Explicit);


            ConfigGraph.DetermineConfigurationType(new FakePolicy3())
                .ShouldEqual(ConfigurationType.Policy);

            ConfigGraph.DetermineConfigurationType(new FakePolicy4())
                .ShouldEqual(ConfigurationType.Explicit);
        }

        [Test]
        public void reorder_policy_is_reording()
        {
            ConfigGraph.DetermineConfigurationType(new ReorderBehaviorsPolicy())
                .ShouldEqual(ConfigurationType.Reordering);
        }


        [Test]
        public void lambda_should_be_indeterminate()
        {
            ConfigGraph.DetermineConfigurationType(new LambdaConfigurationAction(g => { }))
                .ShouldEqual(null);
        }


    }
}