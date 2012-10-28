using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ActionFilter : ActionCallBase
    {
        public static ActionFilter For<T>(Expression<Func<T, object>> method)
        {
            return new ActionFilter(typeof(T), ReflectionHelper.GetMethod(method));
        }

        public ActionFilter(Type handlerType, MethodInfo method) : base(handlerType, method)
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }
    }
}