using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract partial class BehaviorNode
    {
        private ObjectDef _conditionalDef;
        private readonly Guid _uniqueId = Guid.NewGuid();

        public virtual Guid UniqueId
        {
            get { return _uniqueId; }
        }

        protected ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel)
        {
            var objectDef = buildObjectDef();

            if (Next != null)
            {
                var nextObjectDef = Next.As<IContainerModel>().ToObjectDef(diagnosticLevel);
                objectDef.DependencyByType<IActionBehavior>(nextObjectDef);
            }

            if (diagnosticLevel == DiagnosticLevel.FullRequestTracing)
            {
                return createTracerDef(objectDef);
            }


            return objectDef;
        }

        private ObjectDef createTracerDef(ObjectDef objectDef)
        {
            var tracerDef = new ObjectDef(typeof(BehaviorTracer));
            tracerDef.DependencyByType<IActionBehavior>(objectDef);

            var chain = ParentChain();
            tracerDef.DependencyByValue(new BehaviorCorrelation
            {
                ChainId = chain == null ? Guid.Empty : chain.UniqueId,
                BehaviorId = UniqueId
            });

            return tracerDef;
        }

        protected abstract ObjectDef buildObjectDef();

    }
}