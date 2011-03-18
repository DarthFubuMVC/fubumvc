using System;
using FubuCore.Reflection;
using FubuValidation.Strategies;

namespace FubuValidation.Fields
{
    public class GreaterThanZeroRule : IFieldValidationRule
    {
        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue == null) return;
            
            var value = Convert.ToDecimal(rawValue);
            if (value <= 0)
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.GREATER_THAN_ZERO);
            }
        }
    }
}