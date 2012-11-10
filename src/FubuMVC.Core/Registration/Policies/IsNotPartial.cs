using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class IsNotPartial : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            return !chain.IsPartialOnly;
        }
    }
}