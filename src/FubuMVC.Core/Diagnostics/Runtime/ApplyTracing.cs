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
        private static readonly Assembly DiagnosticAssembly = typeof(ApplyTracing).Assembly;

        public void Configure(BehaviorGraph graph)
        {
            // Do nothing
            if (graph.Settings.Get<DiagnosticsSettings>().TraceLevel == TraceLevel.None) return;

            new TracingServices().As<IServiceRegistration>().Apply(graph.Services);

            foreach (BehaviorChain chain in graph.Behaviors.Where(ShouldApply))
            {
                ApplyToChain(chain);
            }
        }

        public static bool ShouldApply(BehaviorChain chain)
        {
            if (chain.IsTagged(BehaviorChain.NoTracing))
            {
                return false;
            }

            // So smelly, but I'm leaving it here.
            if (chain.Calls.Any(x => x.HandlerType.Assembly.GetName().Name == "FubuMVC.Diagnostics"))
            {
                return false;
            }

            if (chain.Calls.Any(x => x.HasAttribute<NoDiagnosticsAttribute>()))
            {
                return false;
            }

            if (chain.InputType() != null && chain.InputType().Assembly == DiagnosticAssembly)
            {
                return false;
            }

            if (chain.ResourceType() != null && chain.ResourceType().Assembly == DiagnosticAssembly)
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