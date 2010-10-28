using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ValidationAttributeSourceTester
    {
        private ValidationAttributeSource _source;

        [SetUp]
        public void BeforeEach()
        {
            _source = new ValidationAttributeSource();
        }

        [Test]
        public void required_attribute_should_resolve_to_field_rule_with_required_strategy()
        {
            _source
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
            _source
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
            _source
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
            _source
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