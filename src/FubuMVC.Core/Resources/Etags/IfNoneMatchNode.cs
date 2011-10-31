using System;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Resources.Etags
{
    public class IfNoneMatchNode : ActionCallBase
    {
        private static readonly string _methodName =
            ReflectionHelper.GetMethod<ETagHandler<IfNoneMatchNode>>(x => x.Matches(null)).Name;

        public IfNoneMatchNode(Type resourceType) : base()
        {
            var handlerType = typeof (ETagHandler<>).MakeGenericType(resourceType);
            var method = handlerType.GetMethod(_methodName);

            setHandlerAndMethod(handlerType, method);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Process; }
        }
    }
}