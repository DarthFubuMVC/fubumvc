using System;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public class ContinuationFieldRule : IFieldValidationRule
    {
        public void Validate(Accessor accessor, ValidationContext context)
        {
            context.ContinueValidation(accessor);
        }
    }
}