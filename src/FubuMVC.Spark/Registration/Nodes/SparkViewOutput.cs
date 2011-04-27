using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<RenderSparkBehavior>
    {
        private static readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly SparkItem _item;

        static SparkViewOutput()
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
        }

        public SparkViewOutput(SparkItem item)
        {
            _item = item;
        }

        protected override void configureObject(ObjectDef def)
        {
            var renderer = def
                .DependencyByType(typeof(ISparkViewRenderer), typeof(SparkViewRenderer));
            
            var action = renderer
                .DependencyByType(typeof(IRenderAction), typeof(RenderAction));
            
            var partialAction = renderer
                .DependencyByType(typeof(IPartialRenderAction), typeof(PartialRenderAction));

            configureProvider(action, createDescriptor(true));
            configureProvider(partialAction, createDescriptor(false));
        }

        private static void configureProvider(ObjectDef actionDef, SparkViewDescriptor descriptor)
        {
            var provider = actionDef
                .DependencyByType(typeof(ISparkViewProvider), typeof(SparkViewProvider));

            provider.DependencyByValue(descriptor);
            provider.DependencyByValue(_cache);            
        }

        private SparkViewDescriptor createDescriptor(bool useMaster)
        {
            var descriptor = new SparkViewDescriptor().AddTemplate(_item.ViewPath);
            if(useMaster && _item.Master != null)
            {
                descriptor.AddTemplate(_item.Master.ViewPath);
            }

            return descriptor;
        }

        public override string Description
        {
            get { return string.Format("Spark View [{0}]", _item.RelativePath()); }
        }
    }
}