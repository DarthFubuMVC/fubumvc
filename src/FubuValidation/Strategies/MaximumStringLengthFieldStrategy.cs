using System.Collections.Specialized;
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

        public NameValueCollection GetMessageSubstitutions(Accessor accessor)
        {
            return new NameValueCollection { { LENGTH,Length.ToString()} };
        }

        public ValidationStrategyResult Validate(ValidationStrategyContext context)
        {
            if (context.RawValue != null && context.RawValue.ToString().Length > Length)
            {
                return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.MAX_LENGTH));
            }

            return ValidationStrategyResult.Valid();
        }
    }
}