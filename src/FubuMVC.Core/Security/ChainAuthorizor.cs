using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security
{
    public class ChainAuthorizor : IChainAuthorizor
    {
        private readonly IFubuRequestContext _context;
        private readonly ITypeResolver _types;

        public ChainAuthorizor(IFubuRequestContext context, ITypeResolver types)
        {
            _context = context;
            _types = types;
        }


        public AuthorizationRight Authorize(BehaviorChain chain, object model)
        {
            if (model != null)
            {
                _context.Models.Set(_types.ResolveType(model), model);
            }

            return chain.Authorization.IsAuthorized(_context);
        }
    }
}