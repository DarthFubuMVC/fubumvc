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

        public static IRouteDefinition BuildRoute(DiagnosticGroup group, ActionCall call)
        {
            if (call.Method.Name == "Index")
            {
                return new RouteDefinition("{0}/{1}".ToFormat(DiagnosticsUrl, group.Url).TrimEnd('/'));
            }

            var route = call.ToRouteDefinition();
            MethodToUrlBuilder.Alter(route, call);
            route.Prepend(@group.Url);
            route.Prepend(DiagnosticsUrl);

            return route;
        }

        public DiagnosticChain(DiagnosticGroup group, ActionCall call) : base(BuildRoute(group, call))
        {
            if (call.HasInput)
            {
                Route.ApplyInputType(call.InputType());
            }

            AddToEnd(call);
        }

        public static DiagnosticChain For<T>(DiagnosticGroup group, Expression<Action<T>> method)
        {
            var call = ActionCall.For(method);
            return new DiagnosticChain(group, call);
        }
    }
}