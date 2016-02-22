using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    public interface IFieldValidationQuery
    {
        IEnumerable<IFieldValidationRule> RulesFor<T>(Expression<Func<T, object>> property);
        IEnumerable<IFieldValidationRule> RulesFor(Type type, Accessor accessor);
        bool HasRule<T>(Type type, Accessor accessor) where T : IFieldValidationRule;
        void ForRule<T>(Type type, Accessor accessor, Action<T> continuation) where T : IFieldValidationRule;
    }

    public class FieldValidationQuery : IFieldValidationQuery
    {
        private readonly IFieldRulesRegistry _registry;

        public FieldValidationQuery(IFieldRulesRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<IFieldValidationRule> RulesFor<T>(Expression<Func<T, object>> property)
        {
            return RulesFor(typeof(T), property.ToAccessor());
        }

        public IEnumerable<IFieldValidationRule> RulesFor(Type type, Accessor accessor)
        {
            var chain = accessor as PropertyChain;
            if (chain == null)
            {
                return _registry.RulesFor(type).RulesFor(accessor);
            }

            if(chainHasValidationContinuedProperties(chain))
            {
                var prop = chain.InnerProperty;
                return _registry.RulesFor(prop.ReflectedType).RulesFor(new SingleProperty(prop));
            }

            return new IFieldValidationRule[0];
        }

        private bool chainHasValidationContinuedProperties(PropertyChain chain)
        {
            if (chain.ValueGetters.Any(x => !(x is PropertyValueGetter))) return false;

            var propertyValueGetters = chain.ValueGetters.OfType<PropertyValueGetter>().Reverse().Skip(1).Reverse();

            return propertyValueGetters.All(x => {
                var accessor = new SingleProperty(x.PropertyInfo);
                return HasRule<ContinuationFieldRule>(accessor.DeclaringType, accessor);
            });
        }

        public bool HasRule<T>(Type type, Accessor accessor) where T : IFieldValidationRule
        {
            return getRules<T>(type, accessor).Any();
        }

        public void ForRule<T>(Type type, Accessor accessor, Action<T> continuation) where T : IFieldValidationRule
        {
            getRules<T>(type, accessor)
                .Each(continuation);
        }

        private IEnumerable<T> getRules<T>(Type type, Accessor accessor)
            where T : IFieldValidationRule
        {
            return RulesFor(type, accessor)
                .OfType<T>();
        }
    }
}