using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class FuncBuilder
    {
        public static object ToFunc(Type concreteType, MethodInfo method)
        {
            // TODO:  Blow up if there's more than one input argument
            // TODO:  Blow up if there's not an output

            var objects = new MethodCallObjects(concreteType, method);

            Type openType = objects.ParameterCount == 2 ? typeof (Func<,>) : typeof (Func<,,>);
            return objects.BuildForOpenType(openType);
        }

        public static object ToAction(Type concreteType, MethodInfo method)
        {
            // TODO:  Blow up if there's more than one input argument
            // TODO:  Blow up if there is an output

            var objects = new MethodCallObjects(concreteType, method);

            Type openType = objects.ParameterCount == 1 ? typeof (Action<>) : typeof (Action<,>);
            return objects.BuildForOpenType(openType);
        }

        private static ParameterExpression toInput(ParameterInfo parameter)
        {
            return Expression.Parameter(parameter.ParameterType, parameter.Name);
        }

        #region Nested type: MethodCallObjects

        public class MethodCallObjects
        {
            private readonly IList<Type> _parameterTypes = new List<Type>();

            public MethodCallObjects(Type concreteType, MethodInfo method)
            {
                ParameterExpression objectParameter = Expression.Parameter(concreteType, "x");

                Parameters = method.GetParameters().Select(x => toInput(x)).ToList();
                MethodCall = Expression.Call(objectParameter, method, Parameters.ToArray());

                Parameters.Insert(0, objectParameter);

                _parameterTypes = new List<Type>();
                _parameterTypes.Add(concreteType);
                _parameterTypes.AddRange(method.GetParameters().Select(x => x.ParameterType));

                _parameterTypes.Add(method.ReturnType);


                _parameterTypes.Remove(typeof (void));
            }

            public IList<ParameterExpression> Parameters { get; private set; }
            public MethodCallExpression MethodCall { get; private set; }
            public Type[] ParameterTypes { get { return _parameterTypes.ToArray(); } }

            public int ParameterCount { get { return _parameterTypes.Count; } }

            public object BuildForOpenType(Type openType)
            {
                Type delegateType = openType.MakeGenericType(ParameterTypes);

                LambdaExpression newExp = Expression.Lambda(delegateType, MethodCall, Parameters);

                return newExp.Compile();
            }
        }

        #endregion
    }
}