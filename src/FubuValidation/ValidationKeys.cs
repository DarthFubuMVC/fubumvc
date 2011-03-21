using FubuCore;
using FubuLocalization;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class ValidationKeys : StringToken
    {
        public static readonly StringToken REQUIRED = new ValidationKeys("REQUIRED", "Required Field");

        public static readonly StringToken COLLECTION_LENGTH 
            = new ValidationKeys("COLLECTION_LENGTH", "Must be exactly {0} element(s)".ToFormat(CollectionLengthRule.LENGTH.AsTemplateField()));

        public static readonly StringToken MAX_LENGTH 
            = new ValidationKeys("MAX_LENGTH", "Maximum length exceeded. Must be less than or equal to {0}".ToFormat(MaximumLengthRule.LENGTH.AsTemplateField()));

        public static readonly StringToken GREATER_THAN_ZERO 
            = new ValidationKeys("GREATER_THAN_ZERO", "Value must be greater than zero");

        public static readonly StringToken GREATER_OR_EQUAL_TO_ZERO 
            = new ValidationKeys("GREATER_OR_EQUAL_TO_ZERO", "Value must be greater than or equal to zero");

        public ValidationKeys(string key)
            : this(key, null)
        {
        }

        public ValidationKeys(string key, string default_EN_US_Text)
            : base(key, default_EN_US_Text)
        {
        }
    }
}