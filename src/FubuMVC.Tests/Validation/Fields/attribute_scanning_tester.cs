using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class attribute_scanning_tester
    {
        private FieldRulesRegistry theRegistry;
        private ClassFieldValidationRules theRules;


        public attribute_scanning_tester()
        {
            theRegistry = new FieldRulesRegistry(new [] { new AttributeFieldValidationSource()},
                                                 new TypeDescriptorCache());

            theRules = theRegistry.RulesFor<AttributeScanningTarget>();
        }

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<AttributeScanningTarget, object>> expression)
        {
            return theRules.RulesFor(expression.ToAccessor());
        }

        [Fact]
        public void found_required_rule()
        {
            rulesFor(x => x.Name).Any(x => x is RequiredFieldRule).ShouldBeTrue();
        }

        [Fact]
        public void found_maximum_string_length_rule()
        {
            rulesFor(x => x.Name).OfType<MaximumLengthRule>().Single().Length.ShouldBe(45);
        }

        [Fact]
        public void found_greater_than_zero()
        {
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Fact]
        public void found_greater_or_equal_to_zero_rule()
        {
            rulesFor(x => x.NumberOfChildren).Single().ShouldBeOfType<GreaterOrEqualToZeroRule>();
        }

        [Fact]
        public void found_collection_length_rule()
        {
            rulesFor(x => x.Names).Single().ShouldBeOfType<CollectionLengthRule>()
                .Length.ShouldBe(2);
        }
    }

    public class AttributeScanningTarget
    {
        [Required, MaximumStringLength(45)]
        public string Name { get; set; }

        [GreaterThanZero]
        public int Age { get; set; }

        [GreaterOrEqualToZero]
        public int NumberOfChildren { get; set; }

        [CollectionLength(2)]
        public string[] Names { get; set; }
    }
}