using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class LambdaFieldValidationSourceTester
    {
        private LambdaFieldValidationSource theSource = null;

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<LambdaFieldValidationSourceTarget, object>> expression)
        {
            var registry = new FieldRulesRegistry(new IFieldValidationSource[]{theSource}, new TypeDescriptorCache());
            return registry.RulesFor<LambdaFieldValidationSourceTarget>(expression);
        }

        [Fact]
        public void invoke_lambda_source_without_specifying_a_filter_should_throw_exception()
        {
            theSource = new LambdaFieldValidationSource(new RequiredFieldRule());

            Exception<FubuValidationException>.ShouldBeThrownBy(() => theSource.As<IFieldValidationSource>().AssertIsValid())
                .Message.ShouldBe("Missing filter on validation convention");
        }

        [Fact]
        public void register_simple_rules_by_filter()
        {
            theSource = new LambdaFieldValidationSource(new RequiredFieldRule());
            theSource.If(a => a.Name.StartsWith("Address"));

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address2).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Fact]
        public void register_simple_rules_by_property_type()
        {
            theSource = new LambdaFieldValidationSource(new GreaterThanZeroRule());
            theSource.IfPropertyType<int>();

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.NullableAge).Any().ShouldBeFalse();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }


        [Fact]
        public void register_simple_rules_by_property_type_filter()
        {
            theSource = new LambdaFieldValidationSource(new GreaterThanZeroRule());
            theSource.IfPropertyType(type => type.IsIntegerBased());

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.NullableAge).Single().ShouldBeOfType<GreaterThanZeroRule>();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Fact]
        public void register_simple_rules_by_rule_func()
        {
            theSource = new LambdaFieldValidationSource(a =>
            {
                var length = int.Parse(a.Name.Substring(7, 1));
                return new MaximumLengthRule(length);
            });

            theSource.If(a => a.Name.StartsWith("Address"));

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldBe(1);
            rulesFor(x => x.Address2).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldBe(2);
        }
    }

    public class LambdaFieldValidationSourceTarget
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public int Age { get; set; }
        public int? NullableAge { get; set; }
    }
}