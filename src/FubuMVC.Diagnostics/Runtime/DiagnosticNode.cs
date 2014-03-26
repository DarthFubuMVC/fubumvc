using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics.Runtime
{
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