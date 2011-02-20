using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation.Registration.Policies
{
    public class EnumerableValidationPolicy : IValidationPolicy
    {
        public bool Matches(Accessor accessor)
        {
            return accessor.PropertyType.IsEnumerable();
        }

        public IEnumerable<IValidationRule> BuildRules(Accessor accessor)
        {
            yield return new EnumerableValidationRule(accessor, new TypeResolver());
        }
    }
}