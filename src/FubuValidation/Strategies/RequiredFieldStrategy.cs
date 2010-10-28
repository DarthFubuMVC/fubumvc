using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class RequiredFieldStrategy : IFieldValidationStrategy
    {
        public static readonly string FIELD = "field";

        public IEnumerable<KeyValuePair<string, string>> GetMessageSubstitutions(Accessor accessor)
        {
            return new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(FIELD, accessor.Name)
                        };
        }

        public ValidationStrategyResult Validate(object target, object rawValue, Type declaringType, Notification notification)
        {
            if(rawValue == null || rawValue.ToString() == string.Empty)
            {
                return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.REQUIRED));
            }

            return ValidationStrategyResult.Valid();
        }
    }
}