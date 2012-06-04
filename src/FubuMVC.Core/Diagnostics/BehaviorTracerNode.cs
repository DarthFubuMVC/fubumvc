using System;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Diagnostics
{
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
}