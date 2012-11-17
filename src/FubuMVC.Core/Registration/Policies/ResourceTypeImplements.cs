using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;

namespace FubuMVC.Core.Registration.Policies
{
    public class ResourceTypeImplements<T> : IChainFilter, DescribesItself
    {
        public bool Matches(BehaviorChain chain)
        {
            if (chain.ResourceType() == null) return false;

            return chain.ResourceType().CanBeCastTo<T>();
        }

        public void Describe(Description description)
        {
            description.Title = "Resource Type can be cast to " + typeof (T).FullName;
        }
    }
}