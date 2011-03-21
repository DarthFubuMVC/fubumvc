using System;
using FubuCore.Reflection;

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

        public bool Equals(GreaterOrEqualToZeroRule other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GreaterOrEqualToZeroRule)) return false;
            return Equals((GreaterOrEqualToZeroRule) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}