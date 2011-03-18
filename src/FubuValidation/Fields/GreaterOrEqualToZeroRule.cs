using System;
using FubuCore.Reflection;
using FubuValidation.Strategies;

namespace FubuValidation.Fields
{
    public class GreaterOrEqualToZeroRule : IFieldValidationRule
    {
        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue != null)
            {
                var value = Convert.ToDouble(rawValue);
                if (value < 0)
                {
                    context.Notification.RegisterMessage(accessor, ValidationKeys.GREATER_OR_EQUAL_TO_ZERO);
                }
            }
        }
    }
}