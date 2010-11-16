using System;
using FubuCore.Reflection;

namespace FubuValidation
{
    public abstract class ValidationAttribute : Attribute
    {
        public abstract IValidationRule CreateRule(Accessor accessor);
    }
}