using System;
using System.Collections.Generic;
using System.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class MaximumStringLengthAttribute : FieldValidationAttribute
    {
        private readonly int _length;

        public MaximumStringLengthAttribute(int length)
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new MaximumLengthRule(_length);
        }
    }
}