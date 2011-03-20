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

        // TODO -- add some stuff back in here for adding strategies


        public RuleConfigurationExpression AddRule(IValidationRule rule)
        {
            _policies.Add(new LambdaValidationPolicy(_filter, rule));
            return this;
        }
    }
}