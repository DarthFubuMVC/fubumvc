using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public interface IPartialFactory
    {
        IActionBehavior BuildPartial(Type inputType);
        IActionBehavior BuildPartial(ActionCall call);
    }

    public class PartialFactory : IPartialFactory
    {
        private readonly ServiceArguments _arguments;
        private readonly ICurrentChain _currentChain;
        private readonly IBehaviorFactory _factory;
        private readonly BehaviorGraph _graph;

        public PartialFactory(BehaviorGraph graph, IBehaviorFactory factory, ServiceArguments arguments, ICurrentChain currentChain)
        {
            _graph = graph;
            _factory = factory;
            _arguments = arguments;
            _currentChain = currentChain;
        }

        public IActionBehavior BuildPartial(Type inputType)
        {
            var behaviorChain = _graph.BehaviorFor(inputType);
            return BuildPartial(behaviorChain);
        }

        public IActionBehavior BuildPartial(ActionCall call)
        {
            var chain = _graph.BehaviorFor(call);
            return BuildPartial(chain);
        }

        private IActionBehavior BuildPartial(BehaviorChain chain)
        {
            var behavior = _factory.BuildBehavior(_arguments, chain.UniqueId);
            return new PartialCurrentChainSwitcher(behavior, _currentChain, chain);
        }
    }

    public class PartialCurrentChainSwitcher : IActionBehavior
    {
        private readonly ICurrentChain _chainStack;
        private readonly BehaviorChain _targetChain;

        public IActionBehavior Inner { get; private set; }

        public PartialCurrentChainSwitcher(IActionBehavior inner, ICurrentChain chainStack, BehaviorChain targetChain)
        {
            Inner = inner;
            _chainStack = chainStack;
            _targetChain = targetChain;
        }

        public void Invoke()
        {
            throw new InvalidOperationException("Not valid unless being used with partial invocation.");
        }

        public void InvokePartial()
        {
            _chainStack.Push(_targetChain);
            Inner.InvokePartial();
            _chainStack.Pop();
        }
    }
}