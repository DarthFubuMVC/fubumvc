using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        

        public void Apply(ActionCall call, IRouteDefinition routeDefinition, IConfigurationObserver observer)
        {
            _httpMethodFilters.Where(x => x.Filter(call)).Each(filter =>
            {
                observer.RecordCallStatus(call,
                    "Adding route constraint {0} based on filter [{1}]".ToFormat(filter.Method, filter.Description));

                routeDefinition.AddHttpMethodConstraint(filter.Method);
            });
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