using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Model
{
    // START HERE!!!!!!
    public class DiagnosticChainsPolicy : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            var diagnosticGraph = new DiagnosticGraph();
            diagnosticGraph.Add(graph.ApplicationAssembly);
            diagnosticGraph.Add(typeof(IPackageFacility).Assembly);

            PackageRegistry.PackageAssemblies.Each(diagnosticGraph.Add);

            graph.Services.AddService(diagnosticGraph);

            return diagnosticGraph.Groups().SelectMany(x => x.Chains);
        }
    }
}