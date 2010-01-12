using System.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

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
            return new RouteDefinition(UrlFor(method));
        }

        public static string UrlFor(MethodInfo method)
        {
            return "_fubu/" + method.Name.ToLower();
        }
    }
}