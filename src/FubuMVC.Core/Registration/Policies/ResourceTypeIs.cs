using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class ResourceTypeIs<T> : IChainFilter, DescribesItself
    {
        public bool Matches(BehaviorChain chain)
        {
            return typeof (T).Equals(chain.ResourceType());
        }

        public void Describe(Description description)
        {
            description.Title = "Resource Type is " + typeof (T).FullName;
        }
    }
}