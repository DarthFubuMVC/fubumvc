using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Infrastructure
{
    public class HttpConstraintResolver : IHttpConstraintResolver
    {
        public const string NoConstraints = "N/A";

        public string Resolve(BehaviorChain chain)
        {
            if (chain.Route == null || !chain.Route.AllowedHttpMethods.Any())
            {
                return NoConstraints;
            }

            return chain.Route.AllowedHttpMethods.Join(",");
        }
    }
}