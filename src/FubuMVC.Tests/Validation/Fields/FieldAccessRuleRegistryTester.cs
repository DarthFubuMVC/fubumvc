using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class when_fetching_rules_for_a_type
    {
        private StubFieldValidationSource<FieldAccessRuleRegistryTarget> source1;
        private StubFieldValidationSource<FieldAccessRuleRegistryTarget> source2;
        private FieldRulesRegistry theRegistry;

        public when_fetching_rules_for_a_type()
        {
            source1 = new StubFieldValidationSource<FieldAccessRuleRegistryTarget>();
            source2 = new StubFieldValidationSource<FieldAccessRuleRegistryTarget>();
        
            source1.Register<RequiredFieldRule>(x => x.Name);
            source2.Register<RequiredFieldRule>(x => x.Name);
            source2.Register<GreaterThanZeroRule>(x => x.Age);
            source1.Register<GreaterOrEqualToZeroRule>(x => x.Children);

            theRegistry = new FieldRulesRegistry(new IFieldValidationSource[]{source1, source2},
                                                      new TypeDescriptorCache());
        }

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<FieldAccessRuleRegistryTarget, object>> property)
        {
            var rules = theRegistry.RulesFor<FieldAccessRuleRegistryTarget>();
            return rules.RulesFor(property.ToAccessor());
        }

        [Fact]
        public void should_not_duplicate_rules()
        {
            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Fact]
        public void should_use_all_sources_to_find_field_rules()
        {
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
            rulesFor(x => x.Children).Single().ShouldBeOfType<GreaterOrEqualToZeroRule>();
        }

        [Fact]
        public void has_rule_positive()
        {
            var accessor = ReflectionHelper.GetAccessor<FieldAccessRuleRegistryTarget>(x => x.Name);
            theRegistry.HasRule<RequiredFieldRule>(accessor).ShouldBeTrue();
        }

        [Fact]
        public void has_rule_negative()
        {
            var accessor = ReflectionHelper.GetAccessor<FieldAccessRuleRegistryTarget>(x => x.Name);
            theRegistry.HasRule<GreaterThanZeroRule>(accessor).ShouldBeFalse(); 
        }

        [Fact]
        public void for_rule_continuation_when_found()
        {
            var accessor = ReflectionHelper.GetAccessor<FieldAccessRuleRegistryTarget>(x => x.Name);
            var action = MockRepository.GenerateMock<Action<RequiredFieldRule>>();
            theRegistry.ForRule<RequiredFieldRule>(accessor, action);

            action.AssertWasCalled(x => x.Invoke(new RequiredFieldRule()));
        }

        [Fact]
        public void for_rule_continuation_should_not_be_called_when_the_rule_can_not_found()
        {
            var accessor = ReflectionHelper.GetAccessor<FieldAccessRuleRegistryTarget>(x => x.Age);
            var action = MockRepository.GenerateMock<Action<RequiredFieldRule>>();
            theRegistry.ForRule(accessor, action);

            action.AssertWasNotCalled(x => x.Invoke(new RequiredFieldRule()));
        }
    }

    public class FieldAccessRuleRegistryTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Children { get; set; }
    }

    public class PropertyHolder
    {
        public PropertyHolder(PropertyInfo property)
        {
            Property = property;
        }

        public PropertyInfo Property{ get; set;}

        public bool Equals(PropertyHolder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Property.PropertyMatches(Property);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyHolder)) return false;
            return Equals((PropertyHolder) obj);
        }

        public override int GetHashCode()
        {
            return (Property != null ? Property.GetHashCode() : 0);
        }
    }

    public class StubFieldValidationSource<T> : IFieldValidationSource
    {
        private readonly Cache<PropertyHolder, IList<IFieldValidationRule>> _rules 
            = new Cache<PropertyHolder, IList<IFieldValidationRule>>(x => new List<IFieldValidationRule>());

        public void Register<TRule>(Expression<Func<T, object>> property) where TRule: IFieldValidationRule, new()
        {
            var holder = new PropertyHolder(property.ToAccessor().InnerProperty);
            _rules[holder].Add(new TRule());
        }

        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            var holder = new PropertyHolder(property);
            return _rules[holder];
        }

        public void AssertIsValid()
        {
            throw new NotImplementedException();
        }
    }
}

