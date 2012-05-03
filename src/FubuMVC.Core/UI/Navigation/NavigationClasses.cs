using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore.Util;
using FubuLocalization;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.UI.Navigation
{
    public interface INavigationService
    {
        IEnumerable<MenuItemToken> MenuFor(StringToken key);
    }

    public interface IMenuStateService
    {
        MenuItemState DetermineStateFor(MenuNode node);
    }

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
            var rights = _authorizor.Authorize(node.BehaviorChain, node.UrlInput);
            if (rights != AuthorizationRight.Allow)
            {
                return node.UnauthorizedState;
            }

            if (_current.OriginatingChain == node.BehaviorChain)
            {
                return MenuItemState.Active;
            }

            if (_conditionals.IsTrue(node.IsEnabledConditionType))
            {
                return MenuItemState.Available;
            }

            return MenuItemState.Disabled;
        }
    }

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

    public class NavigationGraph
    {
        private readonly Cache<StringToken, MenuChain> _chains = new Cache<StringToken, MenuChain>(key => new MenuChain(key));

        public MenuNode FindNode(StringToken key)
        {
            return _chains.FirstValue(x => x.FindByKey(key));
        }

        public MenuChain MenuFor(StringToken key)
        {
            return _chains[key];
        }

        public IEnumerable<MenuNode> AllNodes()
        {
            return _chains.SelectMany(x => x.AllNodes());
        }
    }

    public class NavigationActivator : IActivator
    {
        private readonly IChainResolver _resolver;
        private readonly BehaviorGraph _graph;

        public NavigationActivator(IChainResolver resolver, BehaviorGraph graph)
        {
            _resolver = resolver;
            _graph = graph;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            log.Trace("Trying to resolve chains to the navigation graph");
            _graph.Navigation.AllNodes().Each(x =>
            {
                try
                {
                    x.Resolve(_resolver);
                }
                catch (Exception ex)
                {
                    log.MarkFailure("Failed to resolve a BehaviorChain for navigation element " + x.Key);
                    log.MarkFailure(ex);
                }

            });
        }
    }
}