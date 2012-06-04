using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract partial class BehaviorNode
    {
        private ObjectDef _conditionalDef;

        protected BehaviorNode()
        {
            UniqueId = Guid.NewGuid();
        }

        public virtual Guid UniqueId { get; protected set; }

        public Type BehaviorType
        {
            get
            {
                var objectDef = buildObjectDef();
                return objectDef.Type ?? objectDef.Value.GetType();
            }
        }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            var objectDef = toObjectDef(diagnosticLevel);
            objectDef.Name = UniqueId.ToString();

            return objectDef;
        }

        [MarkedForTermination("Get rid of diagnostic level")]
        protected ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel)
        {
            var objectDef = buildObjectDef();

            if (_conditionalDef != null)
            {
                objectDef = buildConditionalInvokerDef(objectDef);
            }

            if (Next != null)
            {
                attachNextBehavior(objectDef, diagnosticLevel);
            }

            return objectDef;
        }

        private void attachNextBehavior(ObjectDef objectDef, DiagnosticLevel diagnosticLevel)
        {
            var nextObjectDef = Next.As<IContainerModel>().ToObjectDef(diagnosticLevel);
            objectDef.DependencyByType<IActionBehavior>(nextObjectDef);
        }

        private ObjectDef buildConditionalInvokerDef(ObjectDef objectDef)
        {
            var invokerDef = ObjectDef.ForType<ConditionalBehaviorInvoker>();
            var conditionalBehaviorDef = invokerDef
                .DependencyByType<IConditionalBehavior, ConditionalBehavior>();

            conditionalBehaviorDef.DependencyByType<IActionBehavior>(objectDef);
            conditionalBehaviorDef.DependencyByType<IConditional>(_conditionalDef);
            return invokerDef;
        }

        protected abstract ObjectDef buildObjectDef();
    }
}