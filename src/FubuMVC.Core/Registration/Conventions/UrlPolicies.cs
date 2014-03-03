using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlPolicies
    {
        public readonly IList<IUrlPolicy> Policies = new List<IUrlPolicy>
        {
            new UrlPatternAttributePolicy(),
            new DefaultRouteConventionBasedUrlPolicy(),
            new MethodToUrlBuilder(),
            new DefaultUrlPolicy()
        };

        public IRouteDefinition BuildRoute(ActionCall call)
        {
            var policy = Policies.First(x => x.Matches(call));
            return policy.Build(call);
        }
    }
}