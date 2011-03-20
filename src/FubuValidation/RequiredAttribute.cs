using System;
using System.Collections.Generic;
using System.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class RequiredAttribute : FieldValidationAttribute
    {
        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new RequiredFieldRule();
        }
    }
}