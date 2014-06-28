using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticChainsSource : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<DiagnosticsSettings>();

            return settings.Groups.SelectMany(x => x.Chains()).ToArray();
        }
    }
}