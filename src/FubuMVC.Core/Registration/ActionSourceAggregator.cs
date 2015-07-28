using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Web;

namespace FubuMVC.Core.Registration
{
    public class ActionSourceAggregator : IChainSource
    {
        private readonly Assembly _applicationAssembly;
        public readonly IList<IActionSource> Sources = new List<IActionSource>{new EndpointActionSource(), new SendsMessageActionSource()}; 

        public ActionSourceAggregator(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;

            
        }

        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var actions = Sources.SelectMany(x => x.FindActions(_applicationAssembly)).ToArray()
                .Distinct();

            var urlPolicies = graph.Settings.Get<UrlPolicies>();

            return actions.Select(x => x.BuildChain(urlPolicies)).ToArray();
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }
    }
}