using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation
{
    public class ContinuationValidationRule : IValidationRule
    {
        private readonly Accessor _accessor;
        private readonly ITypeResolver _resolver;

        public ContinuationValidationRule(Accessor accessor, ITypeResolver resolver)
        {
            _accessor = accessor;
            _resolver = resolver;
        }

        public void Validate(ValidationContext context)
        {
            var targetValue = _accessor.GetValue(context.Target);

            var targetType = _resolver.ResolveType(targetValue);
            var childNotification = new Notification(targetType);

            context
                .Provider
                .Validate(targetValue, childNotification);

            context.Notification.Include(childNotification);
        }
    }
}