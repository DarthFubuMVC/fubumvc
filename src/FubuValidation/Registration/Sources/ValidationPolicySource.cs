using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuValidation.Registration.Sources
{
    public class ValidationPolicySource : IValidationSource
    {
        private readonly IEnumerable<IValidationPolicy> _policies;
        private readonly Cache<Type, IEnumerable<IValidationRule>> _rules;

        public ValidationPolicySource(IEnumerable<IValidationPolicy> policies)
        {
            _rules = new Cache<Type, IEnumerable<IValidationRule>> { OnMissing = BuildRulesFor };
            _policies = policies;
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            return _rules[type];
        }

        private IEnumerable<IValidationRule> BuildRulesFor(Type type)
        {
            var rules = new List<IValidationRule>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            properties.Each(property =>
                                    {
                                        var accessor = new SingleProperty(property, type);
                                        _policies
                                            .Where(policy => policy.Matches(accessor))
                                            .Each(policy => rules.AddRange(policy.BuildRules(accessor)));
                                    });
            return rules;
        }
    }
}