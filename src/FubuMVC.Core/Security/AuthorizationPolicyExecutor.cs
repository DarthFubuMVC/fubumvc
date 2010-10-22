using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{

    public interface IAuthorizationPolicyExecutor
    {
        AuthorizationRight IsAuthorized(IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies);
    }

    public class AuthorizationPolicyExecutor : IAuthorizationPolicyExecutor
    {
        public virtual AuthorizationRight IsAuthorized(IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies)
        {
            // Check every authorization policy for this endpoint
            var rights = policies.Select(x => x.RightsFor(request));

            // Combine the results
            return AuthorizationRight.Combine(rights);
        }
    }
}