using System.ComponentModel;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    [Description("Diagnostic Node")]
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