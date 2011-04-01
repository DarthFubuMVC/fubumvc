using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;

namespace FubuMVC.Core.Registration.Routes
{
    public interface IRouteInput
    {
        List<RouteParameter> RouteParameters { get; }
        List<RouteParameter> QueryParameters { get; }
        Type InputType { get; }
        int Rank { get; }
        RouteDefinition Parent { get; }
        string CreateUrlFromInput(object input);
        string CreateUrlFromParameters(RouteParameters parameters);
        void AlterRoute(Route route);
        void AddQueryInputs(IEnumerable<RouteParameter> inputs);
        void AddQueryInput(PropertyInfo property);
        void AddRouteInput(RouteParameter parameter, bool appendToUrl);
        RouteParameter RouteInputFor(string routeKey);
        RouteParameter QueryInputFor(string querystringKey);
        string CreateTemplate(object input, Func<object, object>[] hash);
    }
}