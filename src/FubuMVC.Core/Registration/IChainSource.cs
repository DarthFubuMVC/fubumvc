using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IChainSource
    {
        IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph);
    }
}