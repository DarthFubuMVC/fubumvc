using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{

    public class AuthorizationBehavior : BasicBehavior
    {
        // More on this interface below
        private readonly IAuthorizationPolicyExecutor _policyExecutor;
        private readonly IAuthorizationFailureHandler _failureHandler;
        private readonly IFubuRequest _request;
        private readonly IEnumerable<IAuthorizationPolicy> _policies;

        public AuthorizationBehavior(IAuthorizationPolicyExecutor policyExecutor, IAuthorizationFailureHandler failureHandler, IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies) : base(PartialBehavior.Executes)
        {
            _policyExecutor = policyExecutor;
            _failureHandler = failureHandler;
            _request = request;
            _policies = policies;
        }

        protected override DoNext performInvoke()
        {
            var access = _policyExecutor.IsAuthorized(_request, _policies);

            // If authorized, continue to the next behavior in the 
            // chain (filters, controller actions, views, etc.)
            if (access == AuthorizationRight.Allow)
            {
                return DoNext.Continue;
            }

            // If authorization fails, hand off to the failure handler
            // and stop the inner behaviors from executing
            _failureHandler.Handle();
            return DoNext.Stop;
        }

        public ReadOnlyCollection<IAuthorizationPolicy> Policies
        {
            get
            {
                return new ReadOnlyCollection<IAuthorizationPolicy>(_policies.ToList());
            }
        }
    }
}