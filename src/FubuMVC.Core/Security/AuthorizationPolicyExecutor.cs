using System;
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
            return IsAuthorized(request, policies, null);
        }

        protected AuthorizationRight IsAuthorized(IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies, Action<IAuthorizationPolicy, AuthorizationRight> rightsDiscoveryAction)
        {
            // Check every authorization policy for this endpoint
            var rights = policies.Select(policy =>
                                             {
                                                 var policyRights = policy.RightsFor(request);
                                                 
                                                 if(rightsDiscoveryAction != null)
                                                 {
                                                     rightsDiscoveryAction(policy, policyRights);
                                                 }
                                                 
                                                 return policyRights;
                                             });

            // Combine the results
            return AuthorizationRight.Combine(rights);
        }
    }
}