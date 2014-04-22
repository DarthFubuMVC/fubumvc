using System;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;
using System.Collections.Generic;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ConfigurationActionSetTester
    {

        [Test]
        public void latches_duplicate_types_if_they_are_unique()
        {
            var policy1 = new UniquePolicy();
            var policy2 = new UniquePolicy();

            var actions = new ConfigurationActionSet();
            actions.Fill(policy1);

            // policy2 should be treated as a duplicate of policy1
            actions.Fill(policy2);

            actions.Actions.Single().ShouldBeTheSameAs(policy1);

        }

        [Test]
        public void will_not_double_register_actions_that_are_determined_to_be_equivalent()
        {
            var policy1 = new ConfiguredPolicy("foo");
            var policy2 = new ConfiguredPolicy("foo");

            policy1.ShouldEqual(policy2);

            var actions = new ConfigurationActionSet();
            actions.Fill(policy1);

            // policy2 should be treated as a duplicate of policy1
            actions.Fill(policy2);

            actions.Actions.Single().ShouldBeTheSameAs(policy1);
        }

        [Test]
        public void can_register_actions_of_the_same_type_that_are_determined_to_not_be_equivalent()
        {
            var policy1 = new ConfiguredPolicy("foo");
            var policy2 = new ConfiguredPolicy("bar");

            policy1.ShouldNotEqual(policy2);

            var actions = new ConfigurationActionSet();
            actions.Fill(policy1);

            // policy2 should be treated as a duplicate of policy1
            actions.Fill(policy2);

            actions.Actions.ShouldHaveTheSameElementsAs(policy1, policy2);
        }


        [Test]
        public void type_is_unique_is_false_if_the_CanBeMultiplesAttribute_exists()
        {
            ConfigurationActionSet.TypeIsUnique(typeof(SimpleMultiplePolicy)).ShouldBeFalse();
        }

        [Test]
        public void type_is_not_unique_if_there_are_any_constructors_with_parameters()
        {
            ConfigurationActionSet.TypeIsUnique(typeof(ConfiguredPolicy))
                .ShouldBeFalse();
        }

        [Test]
        public void type_is_not_unique_if_there_are_any_settable_properties()
        {
            ConfigurationActionSet.TypeIsUnique(typeof(PropertiesPolicy))
                .ShouldBeFalse();
        }

        [Test]
        public void type_is_unique_if_there_is_no_attribute_no_non_default_ctor_and_no_settable_properties()
        {
            ConfigurationActionSet.TypeIsUnique(typeof(UniquePolicy))
                .ShouldBeTrue();
        }
    }

    [CanBeMultiples]
    public class SimpleMultiplePolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UniquePolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ConfiguredPolicy : IConfigurationAction
    {
        private readonly string _name;

        public ConfiguredPolicy(string name)
        {
            _name = name;
        }

        protected bool Equals(ConfiguredPolicy other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConfiguredPolicy) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public void Configure(BehaviorGraph graph)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PropertiesPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new System.NotImplementedException();
        }

        public string Name { get; set; }
    }
}