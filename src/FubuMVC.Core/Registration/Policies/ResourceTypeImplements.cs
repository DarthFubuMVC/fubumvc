using FubuMVC.Core.Registration.Nodes;
using FubuCore;

namespace FubuMVC.Core.Registration.Policies
{
    public class ResourceTypeImplements<T> : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            if (chain.ResourceType() == null) return false;

            return chain.ResourceType().CanBeCastTo<T>();
        }
    }
}