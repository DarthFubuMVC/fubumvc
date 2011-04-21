using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<SparkViewBehavior>
    {
        private static readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly SparkItem _item;

        static SparkViewOutput()
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
        }
        public SparkViewOutput(SparkItem item) { _item = item; }


        protected override void configureObject(ObjectDef def)
        {
            var dependency = def.EnumerableDependenciesOf<IRenderAction>();
            dependency.AddType(typeof(SetSparkItemAction)).DependencyByValue(_item);
            dependency.AddType(typeof(SetDescriptorAction));
            dependency.AddType(typeof(InvokeRenderAction)).DependencyByType(typeof(IRenderAction), typeof(SetMasterAction));
            dependency.AddType(typeof(SetEntryAction)).DependencyByValue(_cache);
            dependency.AddType(typeof(SetSparkViewInstanceAction));
            dependency.AddType(typeof(ActivateSparkViewAction));
            dependency.AddType(typeof(InvokeRenderAction)).DependencyByType(typeof(IRenderAction), typeof(SetPartialOutputWriterAction));
            dependency.AddType(typeof(InvokePartialRenderAction)).DependencyByType(typeof(IRenderAction), typeof(RenderPartialViewAction));
            dependency.AddType(typeof(InvokeRenderAction)).DependencyByType(typeof(IRenderAction), typeof(RenderViewAction));
            dependency.AddType(typeof(InvokeRenderAction)).DependencyByType(typeof(IRenderAction), typeof(WriteViewOutputAction));
        }

        public override string Description
        {
            get
            {
                return string.Format("Spark View [{0}]", _item.RelativePath());
            }
        }
    }
}