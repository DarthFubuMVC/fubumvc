using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class ContinueValidationAttribute : FieldValidationAttribute
    {
        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new ContinuationFieldRule();
        }
    }
}