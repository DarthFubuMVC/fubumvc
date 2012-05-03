using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuLocalization;

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
}