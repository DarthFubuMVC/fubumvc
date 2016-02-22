using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class FieldValidationAttribute : Attribute, IFieldValidationSource
    {
        public abstract IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);

        public void AssertIsValid()
        {
        }
    }
}