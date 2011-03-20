using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation.Registration.Policies
{
    public class ContinuationValidationPolicy
    {
        public bool Matches(Accessor accessor)
        {
            return !accessor.PropertyType.IsSimple()
                   && !accessor.PropertyType.IsEnumerable();
        }

        public IEnumerable<IValidationRule> BuildRules(Accessor accessor)
        {
            yield return new ContinuationValidationRule(accessor, new TypeResolver());
        }
    }
}