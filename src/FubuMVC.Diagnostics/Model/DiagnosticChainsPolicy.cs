using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics.Model
{
    public class DiagnosticChainsPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var diagnosticGraph = new DiagnosticGraph();
            diagnosticGraph.Add(graph.ApplicationAssembly);
            diagnosticGraph.Add(typeof(IPackageFacility).Assembly);

            PackageRegistry.PackageAssemblies.Each(diagnosticGraph.Add);

            diagnosticGraph.Groups().SelectMany(x => x.Chains).Each(graph.AddChain);

            graph.Services.AddService(diagnosticGraph);
        }
    }
}