using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewNode : OutputNode<RenderSparkBehavior>
    {
        private static readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ViewDescriptor _descriptor;

        static SparkViewNode()
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
        }

        public SparkViewNode(ViewDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        protected override void configureObject(ObjectDef def)
        {
            var renderer = def.DependencyByType(typeof (IViewRenderer), typeof (ViewRenderer));
            var viewDefinition = new ViewDefinition(createSparkDescriptor(true), createSparkDescriptor(false));
            
            var viewEntrySource =
                renderer.DependencyByType(typeof (IRenderAction), typeof (RenderAction))
                .DependencyByType(typeof(IViewFactory), typeof(ViewFactory))
                .DependencyByType(typeof(IViewEntrySource), typeof(ViewEntrySource));

            viewEntrySource.DependencyByValue(_cache);
            viewEntrySource.DependencyByValue(viewDefinition);
        }

        private SparkViewDescriptor createSparkDescriptor(bool useMaster)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(_descriptor.ViewPath);
            if (useMaster && _descriptor.Master != null)
            {
                sparkDescriptor.AddTemplate(_descriptor.Master.ViewPath);
            }

            return sparkDescriptor;
        }

        public override string Description
        {
            get { return string.Format("Spark [{0}]", _descriptor.RelativePath()); }
        }
    }
}