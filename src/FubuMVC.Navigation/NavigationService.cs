using System.Collections.Generic;
using System.Linq;
using FubuLocalization;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;

namespace FubuMVC.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly ICurrentHttpRequest _request;
        private readonly IMenuStateService _stateService;
        private readonly IUrlRegistry _urls;

        private readonly NavigationGraph _navigation;

        public NavigationService(BehaviorGraph graph, ICurrentHttpRequest request, IMenuStateService stateService, IUrlRegistry urls)
        {
            _request = request;
            _stateService = stateService;
            _urls = urls;
            _navigation = graph.Settings.Get<NavigationGraph>();
        }

        public IEnumerable<MenuItemToken> MenuFor(StringToken key)
        {
            var chain = _navigation.MenuFor(key);
            return chain.Select(BuildToken);
        }


        // TODO -- this could really use some more end to end testing
        public MenuItemToken BuildToken(MenuNode node)
        {
            var token = new MenuItemToken {
                Children = node.Children.Select(BuildToken).ToArray(),
                Key = node.Key.Key,
                Text = node.Key.ToString(),
                
                MenuItemState = _stateService.DetermineStateFor(node)
            };

            // TODO -- needs to come back here!  GH-418
            //if (node.Icon().IsNotEmpty())
            //{
            //    token.IconUrl = _urls.UrlForAsset(AssetFolder.images, node.Icon());
            //}

            if (node.Type == MenuNodeType.Leaf)
            {
                token.Url = _request.ToFullUrl(node.CreateUrl());
            }

            return token;
        }
    }
}