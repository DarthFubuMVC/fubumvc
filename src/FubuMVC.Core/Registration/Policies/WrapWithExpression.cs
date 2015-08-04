using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class WrapWithExpression
    {
        private readonly Policy _policy;

        public WrapWithExpression(Policy policy)
        {
            _policy = policy;
        }



 
        public void WithBehavior<T>() where T : IActionBehavior
        {
            _policy.ModifyBy(chain => {
                chain.InsertFirst(Wrapper.For<T>());
            },"Wrap with behavior " + typeof(T).FullName);
        }
    }
}