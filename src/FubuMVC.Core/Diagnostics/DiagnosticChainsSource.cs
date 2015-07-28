using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticChainsSource : IChainSource
    {
        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            var settings = graph.Settings.Get<DiagnosticsSettings>();

            return findActions(graph).ContinueWith(t =>
            {
                return fromActions(settings, t.Result).ToArray();
            });
        }

        private IEnumerable<BehaviorChain> fromActions(DiagnosticsSettings settings, IEnumerable<ActionCall> calls)
        {
            foreach (var action in calls)
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
                    var diagnosticChain = new DiagnosticChain(action);
                    diagnosticChain.Authorization.AddPolicies(settings.AuthorizationRights);

                    yield return diagnosticChain;
                }
            }
        } 

        private Task<ActionCall[]> findActions(BehaviorGraph graph)
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