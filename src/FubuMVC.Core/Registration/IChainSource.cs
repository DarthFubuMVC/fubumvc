using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IChainSource
    {
        Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer);
    }
}