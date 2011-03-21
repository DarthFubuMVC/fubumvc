using System;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public class RequiredFieldRule : IFieldValidationRule
    {
        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);

            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.REQUIRED);
            }
        }

        public bool Equals(RequiredFieldRule other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequiredFieldRule)) return false;
            return Equals((RequiredFieldRule) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}