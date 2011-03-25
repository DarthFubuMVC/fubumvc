namespace FubuValidation
{
    public interface IValidator
    {
        Notification Validate(object target);
        void Validate(object target, Notification notification);
    }
}