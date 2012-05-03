using System.Collections.Generic;
using System.Linq;
using FubuLocalization;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly ICurrentHttpRequest _request;
        private readonly IMenuStateService _stateService;

        private readonly NavigationGraph _navigation;

        public NavigationService(BehaviorGraph graph, ICurrentHttpRequest request, IMenuStateService stateService)
        {
            _request = request;
            _stateService = stateService;
            _navigation = graph.Navigation;
        }

        public IEnumerable<MenuItemToken> MenuFor(StringToken key)
        {
            var chain = _navigation.MenuFor(key);
            return chain.Select(BuildToken);
        }



        public MenuItemToken BuildToken(MenuNode node)
        {
            var token = new MenuItemToken{
                Children = node.Children.Select(BuildToken).ToArray(),
                Key = node.Key.Key,
                Text = node.Key.ToString(),
                
                MenuItemState = _stateService.DetermineStateFor(node)
            };

            if (node.Type == MenuNodeType.Leaf)
            {
                token.Url = _request.ToFullUrl(node.CreateUrl());
            }

            return token;
        }
    }
}