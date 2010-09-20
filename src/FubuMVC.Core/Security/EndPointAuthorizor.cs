using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class EndPointAuthorizor : IEndPointAuthorizor
    {
        private readonly IEnumerable<IAuthorizationPolicy> _policies;

        public EndPointAuthorizor(IEnumerable<IAuthorizationPolicy> policies)
        {
            if (!policies.Any())
            {
                throw new ArgumentOutOfRangeException("policies", "At least one authorization policy is required");    
            }

            _policies = policies;
        }

        public AuthorizationRight IsAuthorized(IFubuRequest request)
        {
            return AuthorizationRight.Combine(_policies.Select(x => x.RightsFor(request)));
        }

        public IEnumerable<string> RulesDescriptions()
        {
            return _policies.Select(x => x.ToString());
        }

        public IEnumerable<IAuthorizationPolicy> Policies
        {
            get { return _policies; }
        }
    }
}