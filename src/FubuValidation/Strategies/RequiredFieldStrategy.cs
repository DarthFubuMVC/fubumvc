using System.Collections.Specialized;
using FubuCore.Reflection;

namespace FubuValidation.Strategies
{
    public class RequiredFieldStrategy : IFieldValidationStrategy
    {
        public static readonly string FIELD = "field";

        public NameValueCollection GetMessageSubstitutions(Accessor accessor)
        {
            return new NameValueCollection { {FIELD, accessor.Name} };
        }

        // TODO -- WTF is ValidationStrategyContext?
        public ValidationStrategyResult Validate(ValidationStrategyContext context)
        {
            if (context.RawValue == null || context.RawValue.ToString() == string.Empty)
            {
                return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.REQUIRED));
            }

            return ValidationStrategyResult.Valid();
        }
    }
}