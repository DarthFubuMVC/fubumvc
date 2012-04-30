using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore.Util;
using FubuLocalization;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationServices : ServiceRegistry
    {
        public NavigationServices()
        {
            AddService<IActivator, NavigationActivator>();
        }
    }

    public class NavigationService
    {
        private readonly ICurrentHttpRequest _request;
        private readonly IChainAuthorizor _authorizor;
        private NavigationGraph _navigation;

        public NavigationService(BehaviorGraph graph, ICurrentHttpRequest request, IChainAuthorizor authorizor)
        {
            _request = request;
            _authorizor = authorizor;
            _navigation = graph.Navigation;
        }

        public MenuItemState DetermineStateFor(MenuNode node)
        {
            var rights = _authorizor.Authorize(node.BehaviorChain, node.UrlInput);
            if (rights != AuthorizationRight.Allow)
            {
                return MenuItemState.Hidden;
            }

            throw new NotImplementedException();
        }

        public MenuItemToken BuildToken(MenuNode node)
        {

            return new MenuItemToken{
                Children = node.Children.Select(BuildToken).ToArray(),
                Key = node.Key.Key,
                Text = node.Key.ToString(),
                Url = node.CreateUrl(),

            };
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