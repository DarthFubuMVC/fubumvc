using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class MinValueAttribute : FieldValidationAttribute
    {
        private readonly IComparable _bounds;

        public MinValueAttribute(int bounds)
        {
            _bounds = bounds;
        }

        public MinValueAttribute(double bounds)
        {
            _bounds = bounds;
        }

        public MinValueAttribute(decimal bounds)
        {
            _bounds = bounds;
        }

        public MinValueAttribute(float bounds)
        {
            _bounds = bounds;
        }

        public override IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
        {
            yield return new MinValueFieldRule(_bounds);
        }
    }
}