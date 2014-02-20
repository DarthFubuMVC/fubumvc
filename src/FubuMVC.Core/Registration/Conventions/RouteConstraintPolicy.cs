using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class RouteConstraintPolicy
    {
        private readonly IList<HttpMethodFilter> _httpMethodFilters = new List<HttpMethodFilter>();


        public void Apply(ActionCall call, IRouteDefinition routeDefinition)
        {
            _httpMethodFilters.Where(x => x.Filter(call)).Each(filter =>
            {
                routeDefinition.AddHttpMethodConstraint(filter.Method);
            });
        }

        public void AddHttpMethodFilter(Expression<Func<ActionCall, bool>> filter, string method)
        {
            _httpMethodFilters.Add(new HttpMethodFilter{
                Filter = filter.Compile(),
                Description = filter.Body.ToString(),
                Method = method
            });
        }

        #region Nested type: HttpMethodFilter

        private struct HttpMethodFilter
        {
            public string Description;
            public Func<ActionCall, bool> Filter;
            public string Method;
        }

        #endregion
    }
}