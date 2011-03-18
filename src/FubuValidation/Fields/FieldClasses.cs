using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuValidation.Fields
{
    /*
     *  Would like to explicitly register field access rules
     *  Query for field access
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */


    public interface IFieldValidationRule
    {
        void Validate(Accessor accessor, ValidationContext context);
    }

    public static class FieldValidationRuleExtensions
    {
        public static Notification Validate<T>(this IFieldValidationRule rule, T target, Expression<Func<T, object>> property)
        {
            var notification = new Notification(typeof (T));
            var accessor = property.ToAccessor();
            var context = new ValidationContext(null, notification, target);

            rule.Validate(accessor, context);

            return notification;
        }
    }

    public interface IFieldValidationSource
    {
        IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);
    }

    // Could have other adapters.
    public class AttributeFieldValidationSource : IFieldValidationSource
    {
        public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            throw new NotImplementedException();
        }
    }

    public class FieldAccessRuleRegistry
    {
        private readonly IEnumerable<IFieldValidationSource> _sources;
        private readonly Cache<Type, ClassFieldValidationRules> _typeRules = new Cache<Type, ClassFieldValidationRules>();

        public FieldAccessRuleRegistry(IEnumerable<IFieldValidationSource> sources)
        {
            _sources = sources;
            _typeRules.OnMissing = findRules;
        }

        private ClassFieldValidationRules findRules(Type type)
        {
            throw new NotImplementedException();
        }
    }

    public class ClassFieldValidationRules : IValidationRule
    {
        private readonly Cache<Accessor, IList<IFieldValidationRule>> _rules = new Cache<Accessor, IList<IFieldValidationRule>>(a => new List<IFieldValidationRule>());

        public void Add(Accessor accessor, IFieldValidationRule rule)
        {
            _rules[accessor].Add(rule);
        }

        public void Validate(ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class CollectionLengthRule : IFieldValidationRule
    {
        public static readonly string FIELD = "field";
        public static readonly string LENGTH = "length";

        public CollectionLengthRule(int length)
        {
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}