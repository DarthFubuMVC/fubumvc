using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class AddToEndExpression
    {
        private readonly Policy _policy;

        public AddToEndExpression(Policy policy)
        {
            _policy = policy;
        }

        public void ToEnd(Func<BehaviorChain, BehaviorNode> source)
        {
            _policy.ModifyBy(chain =>
            {
                var node = source(chain);

                chain.AddToEnd(node);
            });
        }

        public void NodeToEnd<T>() where T : BehaviorNode, new()
        {
            _policy.ModifyBy(chain => chain.AddToEnd(new T()));
        }

        public void BehaviorToEnd<T>() where T : IActionBehavior
        {
            _policy.ModifyBy(chain => chain.AddToEnd(Process.For<T>()));
        }
    }
}