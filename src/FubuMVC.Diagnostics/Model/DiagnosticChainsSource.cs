using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Model
{
    // START HERE!!!!!!
    public class DiagnosticChainsSource : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<DiagnosticsSettings>();

            var diagnosticGraph = new DiagnosticGraph();
            diagnosticGraph.Add(graph.ApplicationAssembly);
            diagnosticGraph.Add(typeof(IPackageFacility).Assembly);

            PackageRegistry.PackageAssemblies.Each(diagnosticGraph.Add);

            graph.Services.AddService(diagnosticGraph);

            var chains = diagnosticGraph.Groups().SelectMany(x => x.Chains).ToArray();

            chains.Each(x => x.Authorization.AddPolicies(settings.AuthorizationRights));

            return chains;
        }
    }
}