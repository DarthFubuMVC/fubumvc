using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    [Title("Not a partial endpoint")]
    public class IsNotPartial : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            return !chain.IsPartialOnly;
        }
    }
}