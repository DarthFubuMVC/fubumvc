using System.Collections.Generic;
using FubuValidation.Registration;
using FubuValidation.Registration.Policies;
using FubuValidation.Registration.Sources;
using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ValidationAttributePolicyTester
    {
        private ValidationPolicySource _policySource;

        [SetUp]
        public void BeforeEach()
        {
            _policySource = new ValidationPolicySource(new List<IValidationPolicy> { new ValidationAttributePolicy() });
        }

        [Test]
        public void required_attribute_should_resolve_to_field_rule_with_required_strategy()
        {
            _policySource
                .RulesFor(typeof(AddressModel))
                .ShouldContain(rule =>
                                   {
                                       var fieldRule = rule as FieldRule;
                                       if (fieldRule == null)
                                       {
                                           return false;
                                       }

                                       return fieldRule.Strategy.GetType() == typeof (RequiredFieldStrategy);
                                   });
        }

        [Test]
        public void greater_than_zero_attribute_should_resolve_to_field_rule_with_greater_than_zero_strategy()
        {
            _policySource
                .RulesFor(typeof(SimpleModel))
                .ShouldContain(rule =>
                                    {
                                        var fieldRule = rule as FieldRule;
                                        if (fieldRule == null)
                                        {
                                            return false;
                                        }

                                        return fieldRule.Strategy.GetType() == typeof(GreaterThanZeroFieldStrategy);
                                    });
        }

        [Test]
        public void greater_or_equal_to_zero_attribute_should_resolve_to_field_rule_with_greater_than_zero_strategy()
        {
            _policySource
                .RulesFor(typeof(SimpleModel))
                .ShouldContain(rule =>
                                    {
                                        var fieldRule = rule as FieldRule;
                                        if (fieldRule == null)
                                        {
                                            return false;
                                        }

                                        return fieldRule.Strategy.GetType() == typeof(GreaterOrEqualToZeroFieldStrategy);
                                    });
        }

        [Test]
        public void maximum_string_length_attribute_should_resolve_to_field_rule_with_maximum_string_length_strategy()
        {
            _policySource
                .RulesFor(typeof(SimpleModel))
                .ShouldContain(rule =>
                                    {
                                        var fieldRule = rule as FieldRule;
                                        if (fieldRule == null)
                                        {
                                            return false;
                                        }

                                        return fieldRule.Strategy.GetType() == typeof(MaximumStringLengthFieldStrategy);
                                    });
        }
    }
}