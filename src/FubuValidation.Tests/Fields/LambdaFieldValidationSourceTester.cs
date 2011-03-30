using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuValidation.Fields;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using FubuCore;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class LambdaFieldValidationSourceTester
    {
        private LambdaFieldValidationSource theSource = null;

        [SetUp]
        public void SetUp()
        {
            theSource = null;
        }

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<LambdaFieldValidationSourceTarget, object>> expression)
        {
            var registry = new FieldRulesRegistry(new IFieldValidationSource[]{theSource}, new TypeDescriptorCache());
            return registry.RulesFor<LambdaFieldValidationSourceTarget>(expression);
        }

        [Test]
        public void invoke_lambda_source_without_specifying_a_filter_should_throw_exception()
        {
            theSource = new LambdaFieldValidationSource(new RequiredFieldRule());

            Exception<FubuValidationException>.ShouldBeThrownBy(() => TypeExtensions.As<IFieldValidationSource>(theSource).Validate())
                .Message.ShouldEqual("Missing filter on validation convention");
        }

        [Test]
        public void register_simple_rules_by_filter()
        {
            theSource = new LambdaFieldValidationSource(new RequiredFieldRule());
            theSource.If(a => a.Name.StartsWith("Address"));

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address2).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Test]
        public void register_simple_rules_by_property_type()
        {
            theSource = new LambdaFieldValidationSource(new GreaterThanZeroRule());
            theSource.IfPropertyType<int>();

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.NullableAge).Any().ShouldBeFalse();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }


        [Test]
        public void register_simple_rules_by_property_type_filter()
        {
            theSource = new LambdaFieldValidationSource(new GreaterThanZeroRule());
            theSource.IfPropertyType(type => type.IsIntegerBased());

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.NullableAge).Single().ShouldBeOfType<GreaterThanZeroRule>();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Test]
        public void register_simple_rules_by_rule_func()
        {
            theSource = new LambdaFieldValidationSource(a =>
            {
                var length = int.Parse(a.Name.Substring(7, 1));
                return new MaximumLengthRule(length);
            });

            theSource.If(a => a.Name.StartsWith("Address"));

            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldEqual(1);
            rulesFor(x => x.Address2).Single().ShouldBeOfType<MaximumLengthRule>().Length.ShouldEqual(2);
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