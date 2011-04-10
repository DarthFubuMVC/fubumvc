using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Diagnostics.Configuration.Policies;
using FubuMVC.Diagnostics.Endpoints;

namespace FubuMVC.Diagnostics.Configuration
{
    public static class FubuExtensions
    {
        private static readonly HashSet<string> HttpVerbs = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "GET", "POST", "PUT", "HEAD" };

        public static void ApplyEndpointConventions(this FubuRegistry registry, params Type[] markerTypes)
        {
            markerTypes
                .Each(t => registry
                               .Applies
                               .ToAssembly(t.Assembly));
            registry
                .Actions
                .IncludeEndpoints(markerTypes);

            registry
                .Routes
                .UrlPolicy<DiagnosticsEndpointUrlPolicy>()
                .UrlPolicy<DiagnosticsAttributeUrlPolicy>();

            registry
                .Routes
                .ConstraintEndpointMethods();
        }

        public static void ConstraintEndpointMethods(this RouteConventionExpression routes)
        {
            HttpVerbs
                .Each(verb => routes.ConstrainToHttpMethod(action => action.Method.Name.Equals(verb, StringComparison.InvariantCultureIgnoreCase), verb));
        }

        public static ActionCallCandidateExpression IncludeEndpoints(this ActionCallCandidateExpression actions, params Type[] markerTypes)
        {
            var markers = new List<Type>(markerTypes);
            markers.Each(markerType => actions.IncludeTypes(t => t.Namespace.StartsWith(markerType.Namespace) && t.Name.EndsWith(DiagnosticsEndpointUrlPolicy.ENDPOINT) && !t.IsAbstract));
            return actions.IncludeMethods(action => HttpVerbs.Contains(action.Method.Name));
        }

        public static void AddRouteInput<T>(this IRouteDefinition route, Expression<Func<T, object>> expression, bool appendToUrl)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            route.Input.AddRouteInput(new RouteParameter(accessor), appendToUrl);
        }

        public static void ApplyRouteInputAttributes(this IRouteDefinition routeDefinition, ActionCall call)
        {
            if (call.HasInput)
            {
                call
                    .InputType()
                    .PropertiesWhere(p => p.HasAttribute<RouteInputAttribute>())
                    .Each(p => routeDefinition.Input.AddRouteInput(new RouteParameter(new SingleProperty(p)), true));
            }
        }

        public static void ApplyQueryStringAttributes(this IRouteDefinition routeDefinition, ActionCall call)
        {
            if (call.HasInput)
            {
                call
                    .InputType()
                    .PropertiesWhere(p => p.HasAttribute<QueryStringAttribute>())
                    .Each(routeDefinition.Input.AddQueryInput);
            }
        }

        public static bool IsDiagnosticsEndpoint(this ActionCall call)
        {
            var diagnosticsAssembly = typeof(DiagnosticsEndpointMarker).Assembly;
            return call.HandlerType.Name.Contains(DiagnosticsEndpointUrlPolicy.ENDPOINT)
                   && call.HandlerType.Assembly.Equals(diagnosticsAssembly)
                   && !call.HasAttribute<FubuDiagnosticsUrlAttribute>();
        }

        public static bool IsDiagnosticsCall(this ActionCall call)
        {
            return call.IsDiagnosticsEndpoint() || call.Method.HasAttribute<FubuDiagnosticsUrlAttribute>();
        }

		public static MethodInfo GetExecuteMethod(this Type type)
		{
			return type.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance);
		}
    }
}