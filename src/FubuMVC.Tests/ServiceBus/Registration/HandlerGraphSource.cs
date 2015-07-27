using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration;

namespace FubuMVC.Tests.ServiceBus.Registration
{
    public class HandlerGraphSource : IChainSource
    {
        public readonly IList<IHandlerSource> HandlerSources = new List<IHandlerSource>();

        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var handlers = new HandlerGraph();
            var allCalls = HandlerSources.SelectMany(x => x.FindCalls(graph.ApplicationAssembly)).Distinct();
            handlers.Add(allCalls);

            handlers.ApplyGeneralizedHandlers();

            return handlers;
        }
    }
}