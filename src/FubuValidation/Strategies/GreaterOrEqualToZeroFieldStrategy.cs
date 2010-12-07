using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class GreaterOrEqualToZeroFieldStrategy : IFieldValidationStrategy
    {
        public IEnumerable<KeyValuePair<string, string>> GetMessageSubstitutions(Accessor accessor)
        {
            return new List<KeyValuePair<string, string>>();
        }

        public ValidationStrategyResult Validate(object target, object rawValue, Type declaringType, Notification notification)
        {
            if(rawValue != null)
            {
                double value = Convert.ToDouble(rawValue);
                if (value < 0)
                {
                    return
                        ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.GREATER_OR_EQUAL_TO_ZERO));
                }
            }

            return ValidationStrategyResult.Valid();
        }
    }
}