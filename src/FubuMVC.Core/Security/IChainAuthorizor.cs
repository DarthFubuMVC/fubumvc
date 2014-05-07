using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security
{
    public interface IChainAuthorizor
    {
        AuthorizationRight Authorize(BehaviorChain chain, object model);
    }
}