using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics.Runtime
{
    [ConfigurationType(ConfigurationType.Instrumentation)]
    public class ApplyTracing : IConfigurationAction
    {
        private static readonly Assembly DiagnosticAssembly = typeof(ApplyTracing).Assembly;

        public void Configure(BehaviorGraph graph)
        {
            // Do nothing
            if (graph.Settings.Get<DiagnosticsSettings>().TraceLevel == TraceLevel.None) return;

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

            if (chain.Calls.Any(x => x.HandlerType.Assembly == DiagnosticAssembly))
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