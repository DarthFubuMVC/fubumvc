using System.Collections.Generic;
using System.Linq;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.UI.Navigation
{
    public class MenuChain : Chain<MenuNode, MenuChain>
    {
        private readonly StringToken _key;

        public MenuChain(StringToken key)
        {
            _key = key;
        }

        public StringToken Key
        {
            get { return _key; }
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