namespace FubuMVC.Core.Validation
{
	// SAMPLE: IValidationRule
    public interface IValidationRule
    {
        void Validate(ValidationContext context);
    }
	// ENDSAMPLE
}