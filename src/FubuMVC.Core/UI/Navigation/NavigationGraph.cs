using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationGraph
    {
        private readonly Cache<StringToken, MenuChain> _chains = new Cache<StringToken, MenuChain>(key => new MenuChain(key));

        public NavigationGraph()
        {
        }

        public NavigationGraph(Action<NavigationRegistry> configure)
        {
            var registry = new NavigationRegistry();
            configure(registry);

            registry.Configure(this);
        }

        public MenuNode FindNode(StringToken key)
        {
            return _chains.FirstValue(x => x.FindByKey(key));
        }

        public MenuChain MenuFor(StringToken key)
        {
            return _chains[key];
        }

        public IEnumerable<IMenuNode> AllNodes()
        {
            foreach (var chain in _chains)
            {
                yield return chain;

                foreach (var node in chain.AllNodes())
                {
                    yield return node;
                }
            }
        }

        public void AddNode(StringToken parentKey, MenuNode node)
        {
            var parentNode = FindNode(parentKey);
            if (parentNode != null)
            {
                parentNode.Children.AddToEnd(node);
            }
            else
            {
                MenuFor(parentKey).AddToEnd(node);
            }
        }

        public IEnumerable<MenuChain> AllMenus()
        {
            return _chains;
        }

        public MenuChain MenuFor(string key)
        {
            return MenuFor(new NavigationKey(key));
        }
    }
}