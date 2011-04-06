using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuValidation.Fields;
using FubuTestingSupport;
using FubuValidation.Tests.Models;
using NUnit.Framework;
using System.Linq;
using FubuCore;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ValidationRegistryTester
    {
        private ValidationRegistry theRegistry = null;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new ValidationRegistry();
        }

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<ValidationRegistryTarget, object>> expression)
        {
            var registry = new FieldRulesRegistry(TypeExtensions.As<IValidationRegistration>(theRegistry).FieldSources(), new TypeDescriptorCache());
            TypeExtensions.As<IValidationRegistration>(theRegistry).RegisterFieldRules(registry);
            
            return registry.RulesFor<ValidationRegistryTarget>(expression);
        }

        [Test]
        public void register_an_explicit_source()
        {
            theRegistry.FieldSource<StubFieldSource>();
            TypeExtensions.As<IValidationRegistration>(theRegistry).FieldSources().Single().ShouldBeOfType<StubFieldSource>();
        }

        [Test]
        public void required_convention()
        {
            theRegistry.Required.IfPropertyType<string>();

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address2).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Age).Any().ShouldBeFalse();
            rulesFor(x => x.NullableAge).Any().ShouldBeFalse();
        }

        [Test]
        public void continuation_convention()
        {
            theRegistry.Continue.IfPropertyType<ContactModel>();
            rulesFor(x => x.Contact).Single().ShouldBeOfType<ContinuationFieldRule>();
        }

        [Test]
        public void greater_than_zero_convention()
        {
            theRegistry.ApplyRule<GreaterThanZeroRule>().IfPropertyType(t => t.IsIntegerBased());

            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
            rulesFor(x => x.NullableAge).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Test]
        public void register_a_validation_rule_with_a_rule_source()
        {
            theRegistry.ApplyRule(a =>
            {
                var length = int.Parse(a.Name.Substring(7, 1));
                return new MaximumLengthRule(length);
            }).If(prop => prop.Name.StartsWith("Address"));

            rulesFor(x => x.Address1).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldEqual(1);
            rulesFor(x => x.Address2).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldEqual(2);
        }

        [Test]
        public void explicitly_register_rules_for_a_class()
        {
            theRegistry.ForClass<ValidationRegistryTarget>(x =>
            {
                x.Require(o => o.Name, o => o.Address1);
            });

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
        
            rulesFor(x => x.Address2).Any().ShouldBeFalse();
        }
    }

    public class StubFieldSource : IFieldValidationSource
    {
        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationRegistryTarget
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public int Age { get; set; }
        public int? NullableAge { get; set; }

        public ContactModel Contact { get; set; }
    }
}