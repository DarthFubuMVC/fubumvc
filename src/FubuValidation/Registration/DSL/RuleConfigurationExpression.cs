using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuValidation.Registration.Policies;

namespace FubuValidation.Registration.DSL
{
    public class RuleConfigurationExpression
    {
        private readonly Func<Accessor, bool> _filter;
        private readonly IList<IValidationPolicy> _policies;

        public RuleConfigurationExpression(Func<Accessor, bool> filter, IList<IValidationPolicy> policies)
        {
            _filter = filter;
            _policies = policies;
        }

        public RuleConfigurationExpression ApplyStrategy<TStrategy>()
            where TStrategy : IFieldValidationStrategy, new()
        {
            return ApplyStrategy<TStrategy>(strategy => { });
        }

        public RuleConfigurationExpression ApplyStrategy<TStrategy>(Action<TStrategy> configure)
            where TStrategy : IFieldValidationStrategy, new()
        {
            _policies.Add(new FieldValidationPolicy<TStrategy>(_filter, configure));
            return this;
        }

        public RuleConfigurationExpression AddRule(IValidationRule rule)
        {
            _policies.Add(new LambdaValidationPolicy(_filter, rule));
            return this;
        }
    }
}