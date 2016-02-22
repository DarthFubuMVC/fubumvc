using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation.Fields
{
	// SAMPLE: IFieldValidationRule
	public interface IFieldValidationRule
	{
		StringToken Token { get; set; }

		// This really only matters for UI purposes but it needs to be a first-class citizen
		ValidationMode Mode { get; set; }

		void Validate(Accessor accessor, ValidationContext context);
	}
	// ENDSAMPLE
}