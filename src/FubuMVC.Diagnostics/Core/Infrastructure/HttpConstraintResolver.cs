using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    [MarkedForTermination]
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