using System.ComponentModel;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Diagnostics.Runtime
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
            instance.Ctor<BehaviorCorrelation>().Is(new BehaviorCorrelation(Next));

            return instance;
        }

        protected override ObjectDef buildObjectDef()
        {
            var tracerDef = new ObjectDef(typeof (BehaviorTracer));

            tracerDef.DependencyByValue(new BehaviorCorrelation(Next));

            return tracerDef;
        }
    }
}