using System;
using System.Linq;

namespace FubuMVC.Core.Registration.Nodes
{
    public static class BehaviorChainExtensions
    {
        public static bool AnyActionHasAttribute<T>(this BehaviorChain chain) where T : Attribute
        {
            return chain.OfType<ActionCall>().Any(x => x.HasAttribute<T>());
        }
    }
}