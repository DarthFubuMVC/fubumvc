using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class MaxValueAttribute : FieldValidationAttribute
    {
        private readonly IComparable _bounds;

        public MaxValueAttribute(int bounds)
        {
            _bounds = bounds;
        }

        public MaxValueAttribute(double bounds)
        {
            _bounds = bounds;
        }

        public MaxValueAttribute(decimal bounds)
        {
            _bounds = bounds;
        }

        public MaxValueAttribute(float bounds)
        {
            _bounds = bounds;
        }

        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new MaxValueFieldRule(_bounds);
        }
    }
}