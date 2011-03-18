using FubuCore.Reflection;
using FubuValidation.Strategies;

namespace FubuValidation.Fields
{
    public class RequiredFieldRule : IFieldValidationRule
    {
        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);

            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.REQUIRED);
            }
        }
    }
}