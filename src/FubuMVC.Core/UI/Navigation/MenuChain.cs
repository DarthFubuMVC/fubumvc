using System;
using System.Collections.Generic;
using System.Linq;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.UI.Navigation
{
    public class MenuChain : Chain<MenuNode, MenuChain>, IMenuNode
    {
        private readonly StringToken _key;

        public MenuChain(string title) : this(new NavigationKey(title))
        {
        }

        public MenuChain(StringToken key)
        {
            _key = key;
        }

        public StringToken Key
        {
            get { return _key; }
        }

        void IMenuNode.AddChild(MenuNode node)
        {
            AddToEnd(node);
        }

        /// <summary>
        /// Find 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MenuNode FindByKey(StringToken key)
        {
            return AllNodes().FirstOrDefault(x => x.Key == key);
        }

        public IEnumerable<MenuNode> AllNodes()
        {
            foreach (var node in this)
            {
                yield return node;

                foreach (var child in node.FindAllChildren())
                {
                    yield return child;
                }
            }
        }
    }
}