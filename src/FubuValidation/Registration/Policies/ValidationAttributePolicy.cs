using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Registration.Policies
{
    public class ValidationAttributePolicy : IValidationPolicy
    {
        public bool Matches(Accessor accessor)
        {
            return accessor.HasAttribute<ValidationAttribute>();
        }

        public IEnumerable<IValidationRule> BuildRules(Accessor accessor)
        {
            var rules = new List<IValidationRule>();
            accessor.ForAttribute<ValidationAttribute>(attr => rules.Add(attr.CreateRule(accessor)));
            return rules;
        }
    }
}