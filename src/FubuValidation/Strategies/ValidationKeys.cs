using FubuCore;
using FubuLocalization;

namespace FubuValidation.Strategies
{
    public class ValidationKeys : StringToken
    {
        public ValidationKeys(string key)
            : this(key, null)
        {    
        }

        public ValidationKeys(string key, string default_EN_US_Text)
            : base(key, default_EN_US_Text)
        {
        }

        public static readonly StringToken REQUIRED = new ValidationKeys("REQUIRED", "{0} is required.".ToFormat(RequiredFieldStrategy.FIELD.AsTemplateField()));
		public static readonly StringToken MAX_LENGTH = new ValidationKeys("MAX_LENGTH", "Maximum length exceeded. Must be less than or equal to {0}.".ToFormat(MaximumStringLengthFieldStrategy.LENGTH.AsTemplateField()));
        public static readonly StringToken GREATER_THAN_ZERO = new ValidationKeys("GREATER_THAN_ZERO", "Value must be greater than zero.");
        public static readonly StringToken GREATER_OR_EQUAL_TO_ZERO = new ValidationKeys("GREATER_OR_EQUAL_TO_ZERO", "Value must be greater than or equal to zero."); 
    }
}