using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class PartialFactory : IPartialFactory
    {
        private readonly ICurrentChain _currentChain;
        private readonly IServiceLocator _services;

        public PartialFactory(IServiceLocator services, ICurrentChain currentChain)
        {
            _services = services;
            _currentChain = currentChain;
        }

        public IActionBehavior BuildBehavior(BehaviorChain chain)
        {
            var behavior = _services.GetInstance<IActionBehavior>(chain.UniqueId.ToString());
            return new FullChainSwitcher(behavior, _currentChain, chain);
        }

        public IActionBehavior BuildPartial(BehaviorChain chain)
        {
            var behavior = _services.GetInstance<IActionBehavior>(chain.UniqueId.ToString());
            return new PartialChainSwitcher(behavior, _currentChain, chain);
        }
    }
}