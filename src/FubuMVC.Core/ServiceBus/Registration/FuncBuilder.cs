using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Core.ServiceBus.Registration
{
    public static class FuncBuilder
    {
        public static object CompileSetter(PropertyInfo property)
        {
            ParameterExpression target = Expression.Parameter(property.ReflectedType, "target");
            ParameterExpression arg = Expression.Parameter(property.PropertyType);
            MethodInfo method = property.GetSetMethod();

            MethodCallExpression callSetMethod = Expression.Call(target, method, arg);

            LambdaExpression lambda = Expression.Lambda(typeof(Action<,>)
                .MakeGenericType(property.ReflectedType, property.PropertyType), callSetMethod, target, arg);

            return lambda.Compile();
        }

        public static object CompileGetter(PropertyInfo property)
        {
            ParameterExpression target = Expression.Parameter(property.ReflectedType, "target");
            MethodInfo method = property.GetGetMethod();

            MethodCallExpression callGetMethod = Expression.Call(target, method);

            LambdaExpression lambda = Expression.Lambda(typeof(Func<,>)
                .MakeGenericType(property.ReflectedType, property.PropertyType), callGetMethod, target);

            return lambda.Compile();
        }
    }
}