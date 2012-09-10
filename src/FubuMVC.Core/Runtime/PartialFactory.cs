using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class PartialFactory : IPartialFactory
    {
        private readonly ServiceArguments _arguments;
        private readonly ICurrentChain _currentChain;
        private readonly IBehaviorFactory _factory;

        public PartialFactory(IBehaviorFactory factory, ServiceArguments arguments, ICurrentChain currentChain)
        {
            _factory = factory;
            _arguments = arguments;
            _currentChain = currentChain;
        }

        public IActionBehavior BuildBehavior(BehaviorChain chain)
        {
            var behavior = _factory.BuildBehavior(_arguments, chain.UniqueId);
            return new FullChainSwitcher(behavior, _currentChain, chain);
        }

        public IActionBehavior BuildPartial(BehaviorChain chain)
        {
            var behavior = _factory.BuildBehavior(_arguments, chain.UniqueId);
            return new PartialChainSwitcher(behavior, _currentChain, chain);
        }
    }
}