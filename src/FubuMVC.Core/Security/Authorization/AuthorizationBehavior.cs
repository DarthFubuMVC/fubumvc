using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
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

        public IEnumerable<IAuthorizationPolicy> Policies => _authorization.Policies;

        protected override async Task invoke(Func<Task> func)
        {
            if (!_settings.AuthorizationEnabled)
            {
                await func().ConfigureAwait(false);
                return;
            }

            var access = _authorization.IsAuthorized(_context);

            // If authorized, continue to the next behavior in the 
            // chain (filters, controller actions, views, etc.)
            if (access == AuthorizationRight.Allow)
            {
                await func().ConfigureAwait(false);
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