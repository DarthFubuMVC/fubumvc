using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class GreaterThanZeroAttribute : FieldValidationAttribute
    {
        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new GreaterThanZeroRule();
        }
    }
}