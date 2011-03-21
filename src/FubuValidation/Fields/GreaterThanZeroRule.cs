using System;
using FubuCore.Reflection;

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

        public bool Equals(GreaterThanZeroRule other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GreaterThanZeroRule)) return false;
            return Equals((GreaterThanZeroRule) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}