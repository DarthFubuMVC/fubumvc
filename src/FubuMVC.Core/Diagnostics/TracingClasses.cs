using System;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    [ConfigurationType(ConfigurationType.Instrumentation)]
    public class ApplyTracing : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            foreach (BehaviorChain chain in graph.Behaviors)
            {
                ApplyToChain(chain);
            }
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

    public class BehaviorTracerNode : BehaviorNode
    {
        public BehaviorTracerNode(BehaviorNode inner)
        {
            inner.AddBefore(this);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Instrumentation; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var tracerDef = new ObjectDef(typeof(BehaviorTracer));

            var chain = ParentChain();
            tracerDef.DependencyByValue(new BehaviorCorrelation
            {
                ChainId = chain == null ? Guid.Empty : chain.UniqueId,
                BehaviorId = Next.UniqueId
            });

            return tracerDef;
        }
    }

    public class DiagnosticNode : BehaviorNode
    {
        public DiagnosticNode(BehaviorChain chain)
        {
            chain.Prepend(this);
        }

        protected override ObjectDef buildObjectDef()
        {
            return new ObjectDef(typeof(DiagnosticBehavior));
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Instrumentation; }
        }
    }


}