using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Json
{
    // TODO -- make this local
    [ApplicationLevel]
    public class JsonBindingSettings
    {
        private readonly ChainPredicate _inclusions = new ChainPredicate();

        public ChainPredicate Include
        {
            get { return _inclusions; }
        }

        public bool ShouldBeIncluded(BehaviorChain chain)
        {
            return _inclusions.As<IChainFilter>().Matches(chain);
        }
    }
}