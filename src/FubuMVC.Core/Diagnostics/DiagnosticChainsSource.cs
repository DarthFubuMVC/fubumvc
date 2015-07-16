using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticChainsSource : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            foreach (var action in findActions(graph))
            {
                if (action.Method.Name.StartsWith("Visualize"))
                {
                    var chain = new BehaviorChain();
                    chain.AddToEnd(action);
                    chain.IsPartialOnly = true;

                    chain.Tags.Add("Diagnostics");

                    yield return chain;
                }
                else
                {
                    yield return new DiagnosticChain(action);
                }
            }
        }

        private IEnumerable<ActionCall> findActions(BehaviorGraph graph)
        {
            var source = new ActionSource();

            source.Applies.ToAssemblyContainingType<IActionBehavior>();
            graph.PackageAssemblies.Each(a => source.Applies.ToAssembly(a));
            source.IncludeTypesNamed(name => name.EndsWith("FubuDiagnostics"));
            source.IncludeTypes(type => type == typeof(FubuDiagnosticsEndpoint));

            return source.As<IActionSource>().FindActions(null);
        }
    }
}