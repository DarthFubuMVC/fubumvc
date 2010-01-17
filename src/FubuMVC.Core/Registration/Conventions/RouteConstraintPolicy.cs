using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    public class RouteConstraintPolicy
    {
        private readonly IList<HttpMethodFilter> _httpMethodFilters = new List<HttpMethodFilter>();
        public static readonly string HTTP_METHOD_CONSTRAINT = "FUBU_HTTP_METHOD_CONSTRAINT";

        public void Apply(ActionCall call, IRouteDefinition routeDefinition)
        {
            var httpMethods = _httpMethodFilters.Where(x => x.Filter(call)).Select(x => x.Method).ToArray();
            if (httpMethods.Length > 0)
            {
                routeDefinition.AddRouteConstraint(HTTP_METHOD_CONSTRAINT, new HttpMethodConstraint(httpMethods));
            }
        }

        public void AddHttpMethodFilter(Func<ActionCall, bool> filter, string method)
        {
            _httpMethodFilters.Add(new HttpMethodFilter{Filter = filter, Method = method});
        }

        private struct HttpMethodFilter
        {
            public Func<ActionCall, bool> Filter;
            public string Method;
        }
    }
}