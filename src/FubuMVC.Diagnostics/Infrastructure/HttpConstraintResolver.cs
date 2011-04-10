using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Infrastructure
{
    public class HttpConstraintResolver : IHttpConstraintResolver
    {
        public const string NoConstraints = "N/A";

        public string Resolve(BehaviorChain chain)
        {
            var constraint = chain
                                 .Route
                                 .Constraints
                                 .Where(kv => kv.Key == RouteConstraintPolicy.HTTP_METHOD_CONSTRAINT)
                                 .Select(kv => kv.Value)
                                 .FirstOrDefault() as HttpMethodConstraint;
            if(constraint == null)
            {
                return NoConstraints;
            }

            return constraint.AllowedMethods.Join(",");
        }
    }
}