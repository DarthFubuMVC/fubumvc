using System.Collections;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation
{
    public class EnumerableValidationRule : IValidationRule
    {
        private readonly Accessor _accessor;
        private readonly ITypeResolver _resolver;

        public EnumerableValidationRule(Accessor accessor, ITypeResolver resolver)
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
            var values = _accessor.GetValue(target) as IEnumerable;
            if(values == null)
            {
                return;
            }

            var targetType = _resolver.ResolveType(values);
            var childNotification = new Notification(targetType);

            values.Each(value => context.Provider.Validate(value, childNotification));
            notification.Include(childNotification);
        }
    }
}