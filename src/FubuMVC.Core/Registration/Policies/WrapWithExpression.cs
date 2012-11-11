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

        public void With(Func<BehaviorChain, BehaviorNode> source)
        {
            _policy.ModifyBy(chain => {
                BehaviorNode node = source(chain);
                chain.InsertFirst(node);
            });
        }

        public void WithNode<T>() where T : BehaviorNode, new()
        {
            _policy.ModifyBy(chain => chain.InsertFirst(new T()));
        }

        public void WithBehavior<T>() where T : IActionBehavior, new()
        {
            _policy.ModifyBy(chain => chain.InsertFirst(Wrapper.For<T>()));
        }
    }
}