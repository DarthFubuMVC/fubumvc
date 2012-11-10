using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class ResourceTypeIs<T> : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            return typeof (T).Equals(chain.ResourceType());
        }
    }
}