using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public interface IChainFilter
    {
        bool Matches(BehaviorChain chain);
    }
}