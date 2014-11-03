using System;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;

namespace FubuMVC.Core.Assets.JavascriptRouting
{
    public class JavascriptRoute
    {
        public string Name;
        public string Method;
        public Func<IChainResolver, BehaviorChain> Finder;

        public RoutedChain FindChain(IChainResolver resolver)
        {
            var chain = Finder(resolver) as RoutedChain;

            if (chain == null)
            {
                throw new Exception("Unable to find a routed chain for a Javascript route named " + Name);
            }
            return chain;
        }
    }
}