using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<RenderSparkViewBehavior>
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
            var descriptor = new SparkViewDescriptor();
            var partialDescriptor = new SparkViewDescriptor();

            descriptor.AddTemplate(_item.ViewPath);
            if (_item.Master != null)
            {
                descriptor.AddTemplate(_item.Master.ViewPath);
            }
            partialDescriptor.AddTemplate(_item.ViewPath);

            var renderer = def.DependencyByType(typeof(ISparkViewRenderer), typeof(SparkViewRenderer));
            var action = renderer.DependencyByType(typeof(IRenderAction), typeof(RenderAction));
            var partialAction = renderer.DependencyByType(typeof(IPartialRenderAction), typeof(PartialRenderAction));

            var provider = action.DependencyByType(typeof (ISparkViewProvider), typeof (SparkViewProvider));
            var partialProvider = partialAction.DependencyByType(typeof (ISparkViewProvider), typeof (SparkViewProvider));

            provider.DependencyByValue(descriptor);
            provider.DependencyByValue(_cache);

            partialProvider.DependencyByValue(partialDescriptor);
            partialProvider.DependencyByValue(_cache);
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