namespace FubuMVC.Core.Validation
{
	// SAMPLE: IValidator
    public interface IValidator
    {
        Notification Validate(object target);
        void Validate(object target, Notification notification);

        ValidationContext ContextFor(object target, Notification notification);
    }
	// ENDSAMPLE
}