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

        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            var sources = _sources.Any() ? _sources : new IActionSource[] {new EndpointActionSource()};

            var actions = sources.SelectMany(x => x.FindActions(_applicationAssembly)).ToArray()
                .Distinct();

            var urlPolicies = graph.Settings.Get<UrlPolicies>();

            return actions.Select(x => x.BuildChain(urlPolicies)).ToArray();
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