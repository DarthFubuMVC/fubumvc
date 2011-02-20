using FubuCore.Reflection;

namespace FubuValidation
{
    public interface IValidationRule
    {
    	bool AppliesTo(Accessor accessor);
        void Validate(object target, ValidationContext context, Notification notification);
    }
}