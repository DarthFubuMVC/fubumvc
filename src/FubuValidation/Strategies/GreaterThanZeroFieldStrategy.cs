using System;
using System.Collections.Specialized;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class GreaterThanZeroFieldStrategy : IFieldValidationStrategy
    {
        public NameValueCollection GetMessageSubstitutions(Accessor accessor)
        {
            return new NameValueCollection();
        }

        public ValidationStrategyResult Validate(ValidationStrategyContext context)
        {
            var rawValue = context.RawValue;
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