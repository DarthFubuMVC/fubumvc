using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation
{
    public abstract class ValidationAttribute : Attribute
    {
        public abstract IValidationRule CreateRule(Accessor accessor);
    }
}