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

        /// <summary>
        /// Explicitly add a BehaviorNode to the end of chains matching this policy 
        /// </summary>
        /// <param name="source"></param>
        public void ToEnd(Func<BehaviorChain, BehaviorNode> source)
        {
            _policy.ModifyBy(chain =>
            {
                var node = source(chain);

                chain.AddToEnd(node);
            });
        }

        /// <summary>
        /// Add a BehaviorNode of type T to the end of this chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void NodeToEnd<T>() where T : BehaviorNode, new()
        {
            _policy.ModifyBy(chain => chain.AddToEnd(new T()));
        }

        /// <summary>
        /// Add a behavior of type T to the end of this chain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void BehaviorToEnd<T>() where T : IActionBehavior
        {
            _policy.ModifyBy(chain => chain.AddToEnd(Process.For<T>()));
        }
    }
}