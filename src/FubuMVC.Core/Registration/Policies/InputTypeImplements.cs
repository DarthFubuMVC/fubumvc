using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class InputTypeImplements<T> : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            if (chain.InputType() == null) return false;

            return chain.InputType().CanBeCastTo<T>();
        }
    }
}