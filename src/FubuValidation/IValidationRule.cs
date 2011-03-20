namespace FubuValidation
{
    public interface IValidationRule
    {
        void Validate(ValidationContext context);
    }
}