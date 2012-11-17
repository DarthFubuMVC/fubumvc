using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class InputTypeImplements<T> : IChainFilter, DescribesItself
    {
        public bool Matches(BehaviorChain chain)
        {
            if (chain.InputType() == null) return false;

            return chain.InputType().CanBeCastTo<T>();
        }

        public void Describe(Description description)
        {
            description.Title = "Input Type can be cast to " + typeof (T).FullName;
        }
    }
}