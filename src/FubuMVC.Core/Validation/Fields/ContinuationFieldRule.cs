using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
    public class ContinuationFieldRule : IFieldValidationRule
    {
	    public StringToken Token { get; set; }

		public ValidationMode Mode { get; set; }

	    public void Validate(Accessor accessor, ValidationContext context)
        {
            context.ContinueValidation(accessor);
        }
    }
}