using System.ComponentModel;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    [Description("Behavior Tracing")]
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

        protected override IConfiguredInstance buildInstance()
        {
            var instance = new SmartInstance<BehaviorTracer>();
            instance.Ctor<BehaviorNode>().Is(Next);

            return instance;
        }
    }
}