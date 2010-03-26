using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    public class RouteConstraintPolicy
    {
        private readonly IList<HttpMethodFilter> _httpMethodFilters = new List<HttpMethodFilter>();
        public static readonly string HTTP_METHOD_CONSTRAINT = "FUBU_HTTP_METHOD_CONSTRAINT";

        public void Apply(ActionCall call, IRouteDefinition routeDefinition, IConfigurationObserver observer)
        {
            var matchingFilters = _httpMethodFilters.Where(x => x.Filter(call));

            var httpMethods = matchingFilters.Select(x => x.Method).ToArray();
            if (httpMethods.Length > 0)
            {
                routeDefinition.AddRouteConstraint(HTTP_METHOD_CONSTRAINT, new HttpMethodConstraint(httpMethods));
                matchingFilters.Each(filter => observer.RecordCallStatus(call,
                    "Adding route constraint {0} based on filter [{1}]".ToFormat(filter.Method, filter.Description)));
                
            }
        }

        public void AddHttpMethodFilter(Expression<Func<ActionCall, bool>> filter, string method)
        {
            _httpMethodFilters.Add(new HttpMethodFilter{Filter = filter.Compile(), Description = filter.Body.ToString(), Method = method});
        }

        private struct HttpMethodFilter
        {
            public Func<ActionCall, bool> Filter;
            public string Description;
            public string Method;
        }
    }
}