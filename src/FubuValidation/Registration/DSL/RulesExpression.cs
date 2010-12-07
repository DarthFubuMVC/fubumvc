using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuValidation.Registration.Policies;

namespace FubuValidation.Registration.DSL
{
    public class RulesExpression
    {
        private readonly IList<IValidationPolicy> _policies;

        public RulesExpression(IList<IValidationPolicy> policies)
        {
            _policies = policies;
        }

        public TypedRuleConfigurationExpression<T> For<T>()
            where T : class
        {
            return new TypedRuleConfigurationExpression<T>(_policies);
        }

        public RuleConfigurationExpression If(Func<Accessor, bool> matches)
        {
            return new RuleConfigurationExpression(matches, _policies);
        }

        public RuleConfigurationExpression IfPropertyIs<T>()
        {
            return If(accessor => accessor.PropertyType == typeof (T));
        }
    }

    public class TypedRuleConfigurationExpression<T>
        where T : class 
    {
        private readonly IList<IValidationPolicy> _policies;

        public TypedRuleConfigurationExpression(IList<IValidationPolicy> policies)
        {
            _policies = policies;
        }

        public TypedRuleConfigurationExpression<T> ApplyStrategy<TStrategy>(Expression<Func<T, object>> expression)
            where TStrategy : IFieldValidationStrategy, new()
        {
            return ApplyStrategy<TStrategy>(expression, strategy => { });
        }

        public TypedRuleConfigurationExpression<T> ApplyStrategy<TStrategy>(Expression<Func<T, object>> expression, Action<TStrategy> configure)
            where TStrategy : IFieldValidationStrategy, new()
        {
            var property = expression.ToAccessor();
            _policies.Add(new FieldValidationPolicy<TStrategy>(accessor => accessor.OwnerType == typeof(T) && accessor.Name == property.Name, configure));
            return this;
        }
    }
}