namespace FubuValidation
{
    public interface IValidationProvider
    {
        Notification Validate(object target);
        void Validate(object target, Notification notification);
    }
}