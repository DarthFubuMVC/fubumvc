using System;

namespace FubuValidation.Strategies
{
    public class ValidationStrategyContext
    {
        public ValidationStrategyContext(object target, object rawValue, Type declaringType,
                                         IValidationProvider provider, Notification notification)
        {
            Target = target;
            RawValue = rawValue;
            DeclaringType = declaringType;
            Provider = provider;
            Notification = notification;
        }

        public object Target { get; private set; }
        public object RawValue { get; private set; }
        public Type DeclaringType { get; private set; }
        public IValidationProvider Provider { get; private set; }
        public Notification Notification { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ValidationStrategyContext)) return false;
            return Equals((ValidationStrategyContext) obj);
        }

        public bool Equals(ValidationStrategyContext other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Target, Target) && Equals(other.RawValue, RawValue) &&
                   Equals(other.DeclaringType, DeclaringType) && Equals(other.Provider, Provider) &&
                   Equals(other.Notification, Notification);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Target != null ? Target.GetHashCode() : 0);
                result = (result*397) ^ (RawValue != null ? RawValue.GetHashCode() : 0);
                result = (result*397) ^ (DeclaringType != null ? DeclaringType.GetHashCode() : 0);
                result = (result*397) ^ (Provider != null ? Provider.GetHashCode() : 0);
                result = (result*397) ^ (Notification != null ? Notification.GetHashCode() : 0);
                return result;
            }
        }
    }
}