namespace FubuValidation
{
    public interface IValidationRule
    {
        void Validate(object target, Notification notification);
    }
}