using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;

namespace FubuMVC.UI.Navigation
{
    public interface IContextualAction<in T>
    {
        string Category { get; }
        MenuItemToken CreateMenuItem(T target);
    }


    public interface IContextActionDefinition<in T>
    {
        string Key { get; }
        string Category { get; }
        MenuItemState UnauthorizedState { get; }
        string Text();
        MenuItemState IsAvailable(T target);
        Endpoint FindEndpoint(IEndpointService endpoints, T target);
    }

    public class AuthorizedContextualAction<T> : IContextualAction<T>
    {
        private readonly IContextActionDefinition<T> _definition;
        private readonly IEndpointService _endpoints;

        public AuthorizedContextualAction(IEndpointService endpoints, IContextActionDefinition<T> definition)
        {
            _endpoints = endpoints;
            _definition = definition;
        }

        public MenuItemToken CreateMenuItem(T target)
        {
            var endpoint = _definition.FindEndpoint(_endpoints, target);

            var menuItemState = determineAvailability(target, endpoint);

            return new MenuItemToken{
                Key = _definition.Key,
                MenuItemState = menuItemState,
                Text = _definition.Text(),
                Url = endpoint.Url
            };
        }

        public string Category
        {
            get { return _definition.Category; }
        }

        private MenuItemState determineAvailability(T target, Endpoint endpoint)
        {
            var authorized = endpoint.IsAuthorized ? MenuItemState.Available : _definition.UnauthorizedState;
            var available = _definition.IsAvailable(target);
            return MenuItemState.Least(authorized, available);
        }
    }

    // TODO -- unit test this like crazy
    public class ContextualMenu<T>
    {
        private readonly IEnumerable<IContextualAction<T>> _actions;

        public ContextualMenu(IEnumerable<IContextualAction<T>> actions)
        {
            _actions = actions;
        }

        public IEnumerable<MenuItemToken> MenuItemsFor(T target)
        {
            return _actions.Select(x => x.CreateMenuItem(target));
        }

        public IEnumerable<MenuItemToken> MenuItemsFor(T target, string category)
        {
            return _actions.Where(x => x.Category == category).Select(x => x.CreateMenuItem(target));
        }
    }
}