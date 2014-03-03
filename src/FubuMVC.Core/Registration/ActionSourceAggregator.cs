using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var actions = sources.SelectMany(x => x.FindActions(_applicationAssembly))
                .Distinct();

            foreach (var action in actions)
            {
                var chain = action.BuildChain();

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