using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class FullChainSwitcher : IActionBehavior
    {
        private readonly ICurrentChain _chainStack;
        private readonly BehaviorChain _targetChain;

        public FullChainSwitcher(IActionBehavior inner,
                                 ICurrentChain chainStack,
                                 BehaviorChain targetChain)
        {
            Inner = inner;
            _chainStack = chainStack;
            _targetChain = targetChain;
        }

        public IActionBehavior Inner { get; private set; }

        public Task Invoke()
        {
            throw new InvalidOperationException("Not valid unless being used with partial invocation.");
        }

        public Task InvokePartial()
        {
            _chainStack.Pop();
            _chainStack.Push(_targetChain);
            return Inner.InvokePartial();
        }
    }
}