using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuValidation.Tests.Strategies
{
    public static class AccessorFactory
    {
        public static Accessor Create<T>(Expression<Func<T, object>> expression)
        {
            return expression.ToAccessor();
        }
    }
}