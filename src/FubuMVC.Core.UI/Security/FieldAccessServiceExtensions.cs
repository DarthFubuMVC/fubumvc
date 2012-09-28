using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuHtml.Security
{
    public static class FieldAccessServiceExtensions
    {
        public static AccessRight RightsFor<T>(this IFieldAccessService fieldAccessService, T model, Expression<Func<T, object>> expression)
        {
            return fieldAccessService.RightsFor(model, expression.ToAccessor().InnerProperty);
        }

        public static bool HasRights<T>(this IFieldAccessService fieldAccessService, T model, Expression<Func<T, object>> expression, AccessRight requiredRights)
        {
            return fieldAccessService.RightsFor(model, expression.ToAccessor().InnerProperty) >= requiredRights;
        }
    }
}