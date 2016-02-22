using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    public interface IFieldValidationSource
    {
        IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);
        void AssertIsValid();
    }
}