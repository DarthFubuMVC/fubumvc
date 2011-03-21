using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuValidation.Fields
{
    public class LambdaFieldValidationSource : IFieldValidationSource
    {
        private readonly Func<PropertyInfo, IFieldValidationRule> _ruleSource;
        private Func<PropertyInfo, bool> _filter;

        public LambdaFieldValidationSource(IFieldValidationRule rule)
        {
            _ruleSource = prop => rule;
        }

        public LambdaFieldValidationSource(Func<PropertyInfo, IFieldValidationRule> ruleSource)
        {
            _ruleSource = ruleSource;
        }

        IEnumerable<IFieldValidationRule> IFieldValidationSource.RulesFor(PropertyInfo property)
        {
            if (!_filter(property))
            {
                yield break;
            }

            yield return _ruleSource(property);
        }

        void IFieldValidationSource.Validate()
        {
            if (_filter == null) throw new FubuValidationException("Missing filter on validation convention");
        }

        public void If(Func<PropertyInfo, bool> filter)
        {
            _filter = filter;
        }

        public void IfPropertyType<T>()
        {
            If(prop => prop.PropertyType == typeof (T));
        }

        public void IfPropertyType(Func<Type, bool> filter)
        {
            If(prop => filter(prop.PropertyType));
        }
    }
}