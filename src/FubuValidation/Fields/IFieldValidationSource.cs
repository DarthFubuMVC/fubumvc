using System.Collections.Generic;
using System.Reflection;

namespace FubuValidation.Fields
{
    public interface IFieldValidationSource
    {
        IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);
        void Validate();
    }
}