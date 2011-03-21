using System;
using System.Collections.Generic;
using System.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class GreaterThanZeroAttribute : FieldValidationAttribute
    {
        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new GreaterThanZeroRule();
        }
    }
}