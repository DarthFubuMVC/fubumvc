using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    [Description("Applies the runtime tracing behaviors to each chain")]
    public class ApplyTracing : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            // Do nothing
            if (graph.Settings.Get<DiagnosticsSettings>().TraceLevel == TraceLevel.None) return;

            new TracingServices().As<IServiceRegistration>().Apply(graph.Services);

            foreach (BehaviorChain chain in graph.Behaviors.Where(ShouldApply).ToArray())
            {
                ApplyToChain(chain);
            }
        }

        public static bool ShouldApply(BehaviorChain chain)
        {
            if (chain is DiagnosticChain) return false;
            if (chain.Tags.Contains("Diagnostics")) return false;

            if (chain.IsTagged(BehaviorChain.NoTracing))
            {
                return false;
            }


            if (chain.Calls.Any(x => x.HasAttribute<NoDiagnosticsAttribute>()))
            {
                return false;
            }

            return true;
        }

        public static void ApplyToChain(BehaviorChain chain)
        {
            var nodes = chain.ToList();
            nodes.Each(x => new BehaviorTracerNode(x));

            if (!chain.IsPartialOnly)
            {
                new DiagnosticNode(chain);
            }
        }
    }
}