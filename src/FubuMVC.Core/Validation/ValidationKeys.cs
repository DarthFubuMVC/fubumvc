using FubuCore;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class ValidationKeys : StringToken
    {
        public static readonly ValidationKeys InvalidFormat = new ValidationKeys("Data is formatted incorrectly");
        public static readonly ValidationKeys Required = new ValidationKeys("Required Field");
        public static readonly ValidationKeys CollectionLength = new ValidationKeys("Must be exactly {0} element(s)".ToFormat(CollectionLengthRule.LENGTH.AsTemplateField()));
        public static readonly ValidationKeys MaxLength = new ValidationKeys("Maximum length exceeded. Must be less than or equal to {0}".ToFormat(MaximumLengthRule.LENGTH.AsTemplateField()));
        public static readonly ValidationKeys MinLength = new ValidationKeys("Minimum length not met. Must be greater than or equal to {0}".ToFormat(MinimumLengthRule.LENGTH.AsTemplateField()));
        public static readonly ValidationKeys GreaterThanZero = new ValidationKeys("Value must be greater than zero");
        public static readonly ValidationKeys GreaterThanOrEqualToZero = new ValidationKeys("Value must be greater than or equal to zero");
        public static readonly ValidationKeys Email = new ValidationKeys("Invalid email address");

		public static readonly ValidationKeys RegEx = new ValidationKeys("Invalid format");

		public static readonly ValidationKeys FieldEquality = new ValidationKeys("{0} must equal {1}".ToFormat(FieldEqualityRule.Field1.AsTemplateField(), FieldEqualityRule.Field2.AsTemplateField()));

        public static readonly ValidationKeys MinValue = new ValidationKeys("Value must be greater than or equal to {bounds}");
        public static readonly ValidationKeys MaxValue = new ValidationKeys("Value must be less than or equal to {bounds}");

        public static readonly ValidationKeys RangeLength = new ValidationKeys("Value must be between {min} and {max} characters");

        public static readonly ValidationKeys Summary = new ValidationKeys("There are errors with the information you provided.");

        public ValidationKeys(string text)
            : base(null, text, namespaceByType: true)
        {   
        }
    }
}