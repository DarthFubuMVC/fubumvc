using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class MaximumStringLengthFieldStrategy : IFieldValidationStrategy
    {
        public static readonly string LENGTH = "length";

        public MaximumStringLengthFieldStrategy()
        {
            Length = 256; // TODO -- what should we default to?
        }

        public int Length { get; set; }

        public IEnumerable<KeyValuePair<string, string>> GetMessageSubstitutions(Accessor accessor)
        {
            return new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(LENGTH, Length.ToString())
                        };
        }

        public ValidationStrategyResult Validate(object target, object rawValue, Type declaringType, Notification notification)
        {
            if (rawValue != null && rawValue.ToString().Length > Length)
            {
                return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.MAX_LENGTH));
            }

            return ValidationStrategyResult.Valid();
        }
    }
}