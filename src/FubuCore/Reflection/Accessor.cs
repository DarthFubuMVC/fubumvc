using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Core.Util
{
    public interface Accessor
    {
        string FieldName { get; }

        Type PropertyType { get; }
        PropertyInfo InnerProperty { get; }
        Type DeclaringType { get; }
        string Name { get; }
        Type OwnerType { get; }
        void SetValue(object target, object propertyValue);
        object GetValue(object target);

        Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression);
    }
}