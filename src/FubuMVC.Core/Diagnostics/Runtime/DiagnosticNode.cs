using System.ComponentModel;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    [Description("Diagnostic Node")]
    public class DiagnosticNode : BehaviorNode
    {
        public DiagnosticNode(BehaviorChain chain)
        {
            chain.Prepend(this);
        }

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<DiagnosticBehavior>();
        }

        protected override ObjectDef buildObjectDef()
        {
            return new ObjectDef(typeof (DiagnosticBehavior));
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Instrumentation; }
        }
    }
}