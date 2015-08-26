using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    [Description("Applies the runtime tracing behaviors to each chain")]
    public class ApplyTracing
    {
        public static void Configure(BehaviorGraph graph)
        {
            foreach (BehaviorChain chain in graph.Chains.Where(ShouldApply).ToArray())
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
        }
    }
}