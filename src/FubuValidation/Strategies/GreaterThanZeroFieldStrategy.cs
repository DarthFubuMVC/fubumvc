using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class GreaterThanZeroFieldStrategy : IFieldValidationStrategy
    {
        public IEnumerable<KeyValuePair<string, string>> GetMessageSubstitutions(Accessor accessor)
        {
            return new List<KeyValuePair<string, string>>();
        }

        public ValidationStrategyResult Validate(object target, object rawValue, Type declaringType, Notification notification)
        {
            if (rawValue != null)
            {
                var value = Convert.ToDecimal(rawValue);
                if (value <= 0)
                {
                    return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.GREATER_THAN_ZERO));
                }
            }

            return ValidationStrategyResult.Valid();
        }
    }
}