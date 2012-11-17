using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    [Title("Partial Chains Only")]
    public class IsPartial : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            return chain.IsPartialOnly;
        }
    }
}