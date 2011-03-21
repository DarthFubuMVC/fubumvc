using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public static class FieldValidationRuleExtensions
    {
        public static Notification ValidateProperty<T>(this IFieldValidationRule rule, T target,
                                               Expression<Func<T, object>> property)
        {
            var notification = new Notification(typeof (T));
            var accessor = property.ToAccessor();
            var context = new ValidationContext(null, notification, target);

            rule.Validate(accessor, context);

            return notification;
        }
    }
}