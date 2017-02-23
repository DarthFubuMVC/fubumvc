using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class ClassValidationRulesTester
    {
        private ClassValidationRules<ClassValidationRulesTarget> theRules = new ClassValidationRules<ClassValidationRulesTarget>();

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<ClassValidationRulesTarget, object>> expression)
        {
            var registry = new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());
            var graph = ValidationGraph.For(registry);

            graph.Import(theRules);

            return registry.RulesFor(typeof (ClassValidationRulesTarget)).RulesFor(expression.ToAccessor());
        }

        private IEnumerable<IValidationRule> classRules()
        {
            var graph = ValidationGraph.For(theRules);

            return graph.Sources.SelectMany(source => source.RulesFor(typeof(ClassValidationRulesTarget)));
        }

        [Fact]
        public void no_rules()
        {
            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Any().ShouldBeFalse();
        }

        [Fact]
        public void register_a_single_required_rule()
        {
            theRules.Require(x => x.Name);

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Fact]
        public void register_a_single_required_rule_with_a_conditional()
        {
            theRules.Require(x => x.Province).If(x => x.Country == "Canada");

            var conditionalRule = rulesFor(x => x.Province).Single().ShouldBeOfType<ConditionalFieldRule<ClassValidationRulesTarget>>();

            conditionalRule.Inner.ShouldBeOfType<RequiredFieldRule>();



            conditionalRule.Condition.Matches(null, ValidationContext.For(new ClassValidationRulesTarget() { Country = "United States" })).ShouldBeFalse();
            conditionalRule.Condition.Matches(null, ValidationContext.For(new ClassValidationRulesTarget() { Country = "Canada" })).ShouldBeTrue();
        }

        [Fact]
        public void register_multiple_required_rules()
        {
            theRules.Require(x => x.Name, x => x.Address1);

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Fact]
        public void register_maximum_length()
        {
            theRules.Property(x => x.Name).MaximumLength(19);
            rulesFor(x => x.Name).Single().ShouldBeOfType<MaximumLengthRule>()
                .Length.ShouldBe(19);
        }

        [Fact]
        public void register_maximum_length_conditionally()
        {
            var filter = FieldRuleCondition.For<ClassValidationRulesTarget>(x => x.Country == "Canada");
            theRules.Property(x => x.Name).MaximumLength(19).If(filter);
            rulesFor(x => x.Name).Single().ShouldBeOfType<ConditionalFieldRule<ClassValidationRulesTarget>>()
                .Inner
                .ShouldBeOfType<MaximumLengthRule>()
                .Length.ShouldBe(19);
        }

        [Fact]
        public void register_greater_than_zero()
        {
            theRules.Property(x => x.Age).GreaterThanZero();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Fact]
        public void register_greater_or_equal_to_zero()
        {
            theRules.Property(x => x.Age).GreaterOrEqualToZero();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterOrEqualToZeroRule>();
        }

        [Fact]
        public void register_required_for_a_single_property()
        {
            theRules.Property(x => x.Name).Required();
        }

        [Fact]
        public void register_multiple_rules_for_a_single_property()
        {
            theRules.Property(x => x.Name).Required().MaximumLength(10);

            var nameRules = rulesFor(x => x.Name);
            CoreExtensions.Count(nameRules).ShouldBe(2);
            nameRules.Any(x => x is RequiredFieldRule).ShouldBeTrue();
            nameRules.ShouldContain(new MaximumLengthRule(10));
        }

        [Fact]
        public void register_class_level_rule_by_type()
        {
            theRules.Register<SimpleClassLevelRule>();

            var classLevelRules = classRules();
            classLevelRules.Any(x => x is SimpleClassLevelRule).ShouldBeTrue();
        }

        [Fact]
        public void register_class_level_rule_by_instance()
        {
            theRules.Register(new ComplexClassLevelRule<ClassValidationRulesTarget>(x => x.Province, x => x.Country));

            var classLevelRules = classRules();
            classLevelRules.Any(x => x is ComplexClassLevelRule<ClassValidationRulesTarget>);
        }

		[Fact]
		public void configure_a_field_equality_rule()
		{
			var myToken = StringToken.FromKeyString("MyKeys:MyToken", "Passwords must match");
			
			theRules
				.Property(x => x.Password)
				.Matches(x => x.ConfirmPassword)
				.ReportErrorsOn(x => x.ConfirmPassword)
				.UseToken(myToken);

			var rule = theRules.As<IValidationSource>().RulesFor(typeof(ClassValidationRulesTarget)).OfType<FieldEqualityRule>().Single();
			
			rule.Property1.ShouldBe(SingleProperty.Build<ClassValidationRulesTarget>(x => x.Password));
			rule.Property2.ShouldBe(SingleProperty.Build<ClassValidationRulesTarget>(x => x.ConfirmPassword));

			rule.Token.ShouldBe(myToken);
		}
    }

    public class ClassValidationRulesTarget
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Province { get; set;}
        public string Country { get; set; }

		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

        public int Age { get; set; }
    }

    public class SimpleClassLevelRule : IValidationRule
    {
        public void Validate(ValidationContext context)
        {
            
        }
    }

    public class ComplexClassLevelRule<T> : IValidationRule where T : class
    {
        private readonly Accessor _firstAccessor;
        private readonly Accessor _secondAccessor;

        public ComplexClassLevelRule(Expression<Func<T, object>> firstField, Expression<Func<T, object>> secondField)
        {
            _firstAccessor = firstField.ToAccessor();
            _secondAccessor = secondField.ToAccessor();
        }


        public void Validate(ValidationContext context)
        {
           
        }
    }
}