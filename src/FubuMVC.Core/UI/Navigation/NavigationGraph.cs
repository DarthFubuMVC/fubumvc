using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuLocalization;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
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

    public class NavigationRegistry : IConfigurationAction
    {
        private readonly IList<Action<NavigationGraph>> _modifications = new List<Action<NavigationGraph>>();
        private StringToken _lastKey;

        // Dru can come back later and change this to suit his OCD while
        // I get stuff done now
        private Action<NavigationGraph> modification
        {
            set
            {
                _modifications.Add(value);
            }
        }

        public void Configure(BehaviorGraph graph)
        {
            _modifications.Each(x => x(graph.Navigation));
        }

        public void Menu(StringToken key)
        {
            _lastKey = key;
        }


        public class InsertExpression
        {
            
        }
    }

    
}