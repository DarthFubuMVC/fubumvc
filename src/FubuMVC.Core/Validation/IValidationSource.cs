using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Validation
{
	// SAMPLE: IValidationSource
    public interface IValidationSource
    {
        IEnumerable<IValidationRule> RulesFor(Type type);
    }
	// ENDSAMPLE
}