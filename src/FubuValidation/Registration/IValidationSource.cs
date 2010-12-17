using System;
using System.Collections.Generic;

namespace FubuValidation.Registration
{
    public interface IValidationSource
    {
		IEnumerable<IValidationRule> RulesFor(Type type);
    }
}