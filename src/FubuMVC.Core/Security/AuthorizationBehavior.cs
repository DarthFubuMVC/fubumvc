using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security
{
    public class AuthorizationBehavior : BasicBehavior
    {
        private readonly IAuthorizationFailureHandler _failureHandler;
        private readonly IFubuRequest _request;
        private readonly IEnumerable<IAuthorizationPolicy> _policies;

        public AuthorizationBehavior(IAuthorizationFailureHandler failureHandler, IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies) : base(PartialBehavior.Executes)
        {
            _failureHandler = failureHandler;
            _request = request;
            _policies = policies;
        }

        protected override DoNext performInvoke()
        {
            var rights = _policies.Select(x => x.RightsFor(_request));
            var access = AuthorizationRight.Combine(rights);

            if (access == AuthorizationRight.Allow)
            {
                return DoNext.Continue;
            }


            _failureHandler.Handle();

            return DoNext.Stop;
        }
    }
}