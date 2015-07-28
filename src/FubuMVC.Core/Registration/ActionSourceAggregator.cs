using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var urlPolicies = graph.Settings.Get<UrlPolicies>();

            var actions = Sources.Select(x => x.FindActions(graph.ApplicationAssembly));

            return Task.WhenAll(actions).ContinueWith(t =>
            {
                return t.Result.SelectMany(x => x).Distinct()
                    .Select(call => call.BuildChain(urlPolicies)).ToArray();
            });
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }
    }
}