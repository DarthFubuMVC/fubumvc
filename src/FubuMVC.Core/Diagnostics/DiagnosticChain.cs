using System;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticChain : RoutedChain
    {
        public const string DiagnosticsUrl = "_fubu";

        public static IRouteDefinition BuildRoute(ActionCall call)
        {
            var prefix = call.HandlerType.Name.Replace("FubuDiagnostics", "").ToLower();

            if (call.Method.Name == "Index")
            {
                return new RouteDefinition("{0}/{1}".ToFormat(DiagnosticsUrl, prefix).TrimEnd('/'));
            }

            var route = call.ToRouteDefinition();
            MethodToUrlBuilder.Alter(route, call);
            route.Prepend(prefix);
            route.Prepend(DiagnosticsUrl);

            return route;
        }

        public DiagnosticChain(ActionCall call) : base(BuildRoute(call))
        {
            if (call.HasInput)
            {
                Route.ApplyInputType(call.InputType());
            }

            RouteName = call.HandlerType.Name.Replace("FubuDiagnostics", "") 
                + ":" 
                + call.Method.Name.Replace("get_", "").Replace("post_", "").Replace("{", "").Replace("}", "");

            AddToEnd(call);
        }

        public static DiagnosticChain For<T>(Expression<Action<T>> method)
        {
            var call = ActionCall.For(method);
            return new DiagnosticChain(call);
        }
    }
}