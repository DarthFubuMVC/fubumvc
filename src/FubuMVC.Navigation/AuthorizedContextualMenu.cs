using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;

namespace FubuMVC.Navigation
{
    public class AuthorizedContextualMenu<T> : IContextualMenu<T>
    {
        private readonly IEndpointService _endpoints;
        private readonly IEnumerable<IContextualAction<T>> _actions;

        public AuthorizedContextualMenu(IEndpointService endpoints, IEnumerable<IContextualAction<T>> actions)
        {
            _endpoints = endpoints;
            _actions = actions;
        }

        public MenuItemToken BuildMenuItem(T target, IContextualAction<T> definition)
        {
            var endpoint = definition.FindEndpoint(_endpoints, target);

            var menuItemState = determineAvailability(target, endpoint, definition);

            return new MenuItemToken{
                Key = definition.Key,
                MenuItemState = menuItemState,
                Text = definition.Text(),
				Category = definition.Category,
				Description = definition.Description(),
                Url = endpoint.Url
            };
        }

        private static MenuItemState determineAvailability(T target, Endpoint endpoint, IContextualAction<T> definition)
        {
            var authorized = endpoint.IsAuthorized ? MenuItemState.Available : definition.UnauthorizedState;
            var available = definition.IsAvailable(target);
            return MenuItemState.Least(authorized, available);
        }

        public IEnumerable<MenuItemToken> MenuItemsFor(T target)
        {
            return _actions.Select(x => BuildMenuItem(target, x));
        }

        public IEnumerable<MenuItemToken> MenuItemsFor(T target, string category)
        {
            return _actions.Where(x => x.Category == category).Select(x => BuildMenuItem(target, x));
        }
    }

}