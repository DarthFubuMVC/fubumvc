using System.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticUrlPolicy : IUrlPolicy
    {
        public bool Matches(ActionCall call)
        {
            return call.HandlerType == typeof (BehaviorGraphWriter);
        }

        public IRouteDefinition Build(ActionCall call)
        {
            MethodInfo method = call.Method;
            var definition = call.ToRouteDefinition();
            definition.Append("_fubu/" + UrlFor(method));
            if (call.InputType().CanBeCastTo<ChainRequest>())
            {
                definition.AddRouteInput(new RouteInput(ReflectionHelper.GetAccessor<ChainRequest>(x => x.Id)), true);
            }
            return definition;
        }

        public static string UrlFor(MethodInfo method)
        {
            return method.Name.ToLower();
        }

        public static string RootUrlFor(MethodInfo method)
        {
            return "_fubu/" + method.Name.ToLower();
        }

    }
}