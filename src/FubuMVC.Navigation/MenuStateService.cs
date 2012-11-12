using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Security;

namespace FubuMVC.Navigation
{
    public class MenuStateService : IMenuStateService
    {
        private readonly IChainAuthorizor _authorizor;
        private readonly ICurrentChain _current;
        private readonly IConditionalService _conditionals;

        public MenuStateService(IChainAuthorizor authorizor, ICurrentChain current, IConditionalService conditionals)
        {
            _authorizor = authorizor;
            _current = current;
            _conditionals = conditionals;
        }

        public virtual MenuItemState DetermineStateFor(MenuNode node)
        {
            if (node.BehaviorChain == null)
            {
                return MenuItemState.Available;
            }

            var rights = _authorizor.Authorize(node.BehaviorChain, node.UrlInput);
            if (rights != AuthorizationRight.Allow)
            {
                return node.UnauthorizedState;
            }

            if (_current.OriginatingChain == node.BehaviorChain)
            {
                return MenuItemState.Active;
            }

            if (_conditionals.IsTrue(node.HideIfConditional))
            {
                return MenuItemState.Hidden;
            }

            if (_conditionals.IsTrue(node.IsEnabledBy()))
            {
                return MenuItemState.Available;
            }

            return MenuItemState.Disabled;
        }
    }
}