using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using StructureMap;

namespace FubuMVC.Core.ServerSentEvents
{
    public class TopicChainSource : IChainSource
    {
        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var types = TypeRepository.FindTypes(graph.AllAssemblies(), TypeClassification.Concretes,
                type => type.CanBeCastTo<Topic>());

            return types.ContinueWith(t =>
            {
                return t.Result.Select(x => new SseTopicChain(x).As<BehaviorChain>()).ToArray();
            });

        }
    }
}