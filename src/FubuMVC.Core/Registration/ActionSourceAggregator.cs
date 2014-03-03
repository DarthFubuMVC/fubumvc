using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class ActionSourceAggregator : IChainSource
    {
        private readonly Assembly _applicationAssembly;
        private readonly IList<IActionSource> _sources = new List<IActionSource>(); 

        public ActionSourceAggregator(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
        }

        public IEnumerable<BehaviorChain> BuildChains(SettingsCollection settings)
        {
            var sources = _sources.Any() ? _sources : new IActionSource[] {new EndpointActionSource()};

            var actions = sources.SelectMany(x => x.FindActions(_applicationAssembly)).ToArray()
                .Distinct();

            var urlPolicies = settings.Get<UrlPolicies>();

            return findChains(actions, urlPolicies).ToArray();
        }

        private static IEnumerable<BehaviorChain> findChains(IEnumerable<ActionCall> actions, UrlPolicies urlPolicies)
        {
            foreach (var action in actions)
            {
                // TODO -- will take in UrlPolicies later
                var chain = action.BuildChain();

                if (!chain.IsPartialOnly)
                {
                    chain.Route = urlPolicies.BuildRoute(action);
                }

                yield return chain;
            }
        }

        public void Add(IActionSource source)
        {
            _sources.Add(source);
        }

        public IEnumerable<IActionSource> Sources
        {
            get { return _sources; }
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }
    }
}