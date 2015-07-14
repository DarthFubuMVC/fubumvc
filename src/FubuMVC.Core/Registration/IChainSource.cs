using System.Collections.Generic;
using Bottles.Diagnostics;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IChainSource
    {
        IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph, IPerfTimer timer);
    }
}