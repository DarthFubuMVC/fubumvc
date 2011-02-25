using System;
using System.Collections.Specialized;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class GreaterOrEqualToZeroFieldStrategy : IFieldValidationStrategy
    {
        public NameValueCollection GetMessageSubstitutions(Accessor accessor)
        {
            return new NameValueCollection();
        }

        public ValidationStrategyResult Validate(ValidationStrategyContext context)
        {
            var rawValue = context.RawValue;
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