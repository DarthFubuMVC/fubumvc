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

        public bool AppliesTo(Accessor accessor)
        {
            return _accessor.Equals(accessor);
        }

        public void Validate(object target, ValidationContext context, Notification notification)
        {
            var targetValue = _accessor.GetValue(target);

            var targetType = _resolver.ResolveType(targetValue);
            var childNotification = new Notification(targetType);

            context
                .Provider
                .Validate(targetValue, childNotification);

            notification.Include(childNotification);
        }
    }
}