using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Navigation
{
    public class NavigationService : INavigationService
    {
	    private readonly IChainUrlResolver _urlResolver;
        private readonly IMenuStateService _stateService;
        private readonly IAssetTagBuilder _urls;

        private readonly IMenuResolver _navigation;

        public NavigationService(IMenuResolver navigation, IChainUrlResolver urlResolver, IMenuStateService stateService, IAssetTagBuilder urls)
		{
            _navigation = navigation;
            _urlResolver = urlResolver;
            _stateService = stateService;
            _urls = urls;
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
                Category = node.Category,
                MenuItemState = _stateService.DetermineStateFor(node)
            };

            if (node.Icon().IsNotEmpty())
            {
                token.IconUrl = _urls.FindImageUrl(node.Icon());
            }

            if (node.Type == MenuNodeType.Leaf)
            {
	            token.Url = _urlResolver.UrlFor(node.UrlInput, node.BehaviorChain);
            }

			node.ForData(token.Set);

            return token;
        }
    }
}