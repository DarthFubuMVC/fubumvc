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

        public void With(Func<BehaviorChain, BehaviorNode> source, string description = "Wrap with a BehaviorNode built by a Lambda")
        {
            _policy.ModifyBy(chain => {
                BehaviorNode node = source(chain);
                chain.InsertFirst(node);
            }, description);
        }

        public void WithNode<T>() where T : BehaviorNode, new()
        {
            _policy.ModifyBy(chain => chain.InsertFirst(new T()), "Wrap with node " + typeof(T).FullName);
        }

        public void WithBehavior<T>() where T : IActionBehavior
        {
            _policy.ModifyBy(chain => {
                chain.InsertFirst(Wrapper.For<T>());
            },"Wrap with behavior " + typeof(T).FullName);
        }
    }
}