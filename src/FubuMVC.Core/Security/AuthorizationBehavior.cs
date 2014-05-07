using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Security
{
    public class AuthorizationBehavior : BasicBehavior
    {
        // More on this interface below
        private readonly IAuthorizationNode _authorization;
        private readonly IFubuRequestContext _context;
        private readonly IAuthorizationFailureHandler _failureHandler;

        public AuthorizationBehavior(IAuthorizationNode authorization, IFubuRequestContext context,
            IAuthorizationFailureHandler failureHandler) : base(PartialBehavior.Executes)
        {
            _authorization = authorization;
            _context = context;
            _failureHandler = failureHandler;
        }

        public IEnumerable<IAuthorizationPolicy> Policies
        {
            get
            {
                return _authorization.Policies;
            }
        }

        protected override DoNext performInvoke()
        {
            var access = _authorization.IsAuthorized(_context);

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
    }
}