using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuValidation
{
    public class ValidationAttributeSource : IValidationSource
    {
        private readonly Cache<Type, IEnumerable<IValidationRule>> _rules;

        public ValidationAttributeSource()
        {
            _rules = new Cache<Type, IEnumerable<IValidationRule>> { OnMissing = BuildRulesFor };
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            return _rules[type];
        }

        private IEnumerable<IValidationRule> BuildRulesFor(Type type)
        {
            var rules = new List<IValidationRule>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            properties
                .Each(property =>
                          {
                              var accessor = new SingleProperty(property, type);
                              property
                                  .GetCustomAttributes(true)
                                  .Where(attribute => typeof (ValidationAttribute).IsAssignableFrom(attribute.GetType()))
                                  .Cast<ValidationAttribute>()
                                  .Each(attribute => rules.Add(attribute.CreateRule(accessor)));
                          });

            return rules;
        }
    }
}