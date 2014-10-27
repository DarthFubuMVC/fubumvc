using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security
{
    // Mostly tested through integration tests now
    public class AuthorizationBehavior : WrappingBehavior
    {
        // More on this interface below
        private readonly IAuthorizationNode _authorization;
        private readonly IFubuRequestContext _context;
        private readonly IAuthorizationFailureHandler _failureHandler;
        private readonly SecuritySettings _settings;

        public AuthorizationBehavior(IAuthorizationNode authorization, IFubuRequestContext context, IAuthorizationFailureHandler failureHandler, SecuritySettings settings)
        {
            _authorization = authorization;
            _context = context;
            _failureHandler = failureHandler;
            _settings = settings;
        }

        public IEnumerable<IAuthorizationPolicy> Policies
        {
            get
            {
                return _authorization.Policies;
            }
        }

        protected override void invoke(Action action)
        {
            if (!_settings.AuthorizationEnabled)
            {
                action();
                return;
            }

            var access = _authorization.IsAuthorized(_context);

            // If authorized, continue to the next behavior in the 
            // chain (filters, controller actions, views, etc.)
            if (access == AuthorizationRight.Allow)
            {
                action();
            }
            else
            {
                // If authorization fails, hand off to the failure handler
                // and stop the inner behaviors from executing
                var continuation = _failureHandler.Handle();
                _context.Service<IContinuationProcessor>().Continue(continuation, Inner); 
            }


        }
    }
}