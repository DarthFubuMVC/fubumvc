using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Tests.Registration.Utilities
{
    public class ActionCallSpec : ChainedBehaviorSpec<ActionCall>
    {
        private readonly MethodInfo _method;
        private readonly Type _type;

        public ActionCallSpec(Type type, MethodInfo method)
        {
            _type = type;
            _method = method;
        }

        public static ActionCallSpec For<T>(Expression<Func<T, object>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return new ActionCallSpec(typeof (T), method);
        }

        public static ActionCallSpec For<T>(Expression<Action<T>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return new ActionCallSpec(typeof (T), method);
        }

        protected override void doSpecificCheck(ActionCall call, BehaviorSpecCheck check)
        {
            if (call.HandlerType != _type)
            {
                check.RegisterError("type was " + call.HandlerType.Name);
            }

            if (call.Method != _method)
            {
                check.RegisterError("method was " + call.Method.Name);
            }
        }


        public override string ToString()
        {
            return "Call {0}.{1}".ToFormat(_type.Name, _method.Name);
        }
    }
}