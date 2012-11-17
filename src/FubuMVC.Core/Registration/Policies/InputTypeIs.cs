using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class InputTypeIs<T> : IChainFilter, DescribesItself
    {
        public bool Matches(BehaviorChain chain)
        {
            return typeof (T).Equals(chain.InputType());
        }

        public void Describe(Description description)
        {
            description.Title = "Input Type is " + typeof (T).FullName;
        }
    }
}