using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.DSL
{
    public class TypeMethodPolicy<T>
    {
        private readonly CompositeFilter<MethodInfo> _filter;

        public TypeMethodPolicy(CompositeFilter<MethodInfo> filter)
        {
            _filter = filter;
        }

        public void Include(Expression<Action<T>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            _filter.Includes += m => m.Name == method.Name;
        }

        public void IncludeMethods(Func<MethodInfo, bool> filter)
        {
            _filter.Includes += filter;
        }

        public void Exclude(Expression<Action<T>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            _filter.Excludes += m => m.Name == method.Name;
        }
    }
}