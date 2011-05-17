using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Diagnostics.Infrastructure
{
    public class HttpConstraintResolver : IHttpConstraintResolver
    {
        public const string NoConstraints = "N/A";

        public string Resolve(BehaviorChain chain)
        {
            var constraint = chain
                                 .Route
                                 .Constraints[RouteDefinition.HTTP_METHOD_CONSTRAINT] as HttpMethodConstraint;
            if(constraint == null)
            {
                return NoConstraints;
            }

            return constraint.AllowedMethods.Join(",");
        }
    }
}