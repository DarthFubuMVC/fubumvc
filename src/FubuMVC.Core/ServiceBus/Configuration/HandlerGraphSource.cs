using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Registration;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    // TODO -- add perf timings
    public class HandlerGraphSource : IChainSource
    {
        public readonly IList<IHandlerSource> HandlerSources = new List<IHandlerSource>{new DefaultHandlerSource()};

        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var finders = HandlerSources.Select(x => x.FindCalls(graph.ApplicationAssembly)).ToArray();
            return Task.WhenAll(finders).ContinueWith(all =>
            {
                var handlers = new HandlerGraph();

                var allCalls = all.Result.SelectMany(x => x).Distinct();

                handlers.Add(allCalls);

                handlers.Compile();

                return handlers.OfType<BehaviorChain>().ToArray();
            });
        }
    }
}