using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuValidation.Tests.Rules
{
    public class Property
    {
        public static PropertyInfo From<T>(Expression<Func<T, object>> property)
        {
            return ReflectionHelper.GetProperty(property);
        }
    }
}