using FubuCore.Reflection;

namespace FubuValidation
{
    public interface IValidationRule
    {
    	bool AppliesTo(Accessor accessor);
        void Validate(object target, Notification notification);
    }
}