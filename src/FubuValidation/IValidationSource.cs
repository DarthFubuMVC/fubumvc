using System;
using System.Collections.Generic;

namespace FubuValidation
{
    public interface IValidationSource
    {
        IEnumerable<IValidationRule> RulesFor(Type type);
    }
}