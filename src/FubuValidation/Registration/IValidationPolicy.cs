using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Registration
{
    public interface IValidationPolicy
    {
        bool Matches(Accessor accessor);
        IEnumerable<IValidationRule> BuildRules(Accessor accessor);
    }
}