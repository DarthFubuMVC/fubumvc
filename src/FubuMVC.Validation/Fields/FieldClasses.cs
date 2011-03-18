using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuValidation;

namespace FubuMVC.Validation.Fields
{
    public interface IFieldValidationRule
    {
        void Validate(Accessor accessor, ValidationContext context);
    }

    public interface IFieldValidationAttribute
    {
        IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property);        
    }


}