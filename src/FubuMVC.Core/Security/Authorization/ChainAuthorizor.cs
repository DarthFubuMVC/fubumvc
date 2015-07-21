using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authorization
{
    public class ChainAuthorizor : IChainAuthorizor
    {
        private readonly IFubuRequestContext _context;

        public ChainAuthorizor(IFubuRequestContext context)
        {
            _context = context;
        }

        public AuthorizationRight Authorize(BehaviorChain chain, object model)
        {
            if (model != null)
            {
                _context.Models.Set(model.GetType(), model);
            }

            return chain.Authorization.IsAuthorized(_context);
        }
    }
}