using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewNode : OutputNode<RenderSparkBehavior>, IMayHaveInputType
    {
        private readonly ViewDescriptor _descriptor;

        public SparkViewNode(ViewDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        protected override void configureObject(ObjectDef def)
        {
            var renderer = def.DependencyByType(typeof (IViewRenderer), typeof (ViewRenderer));

            var viewEntrySource = renderer.DependencyByType(typeof(IRenderAction), typeof(RenderAction))
                .DependencyByType(typeof(IViewFactory), typeof(ViewFactory))
                .DependencyByType(typeof(IViewEntrySource), typeof(ViewEntrySource));

            viewEntrySource.DependencyByValue(_descriptor);
        }

        public override string Description
        {
            get { return string.Format("Spark [{0}]", _descriptor.RelativePath()); }
        }

        public Type InputType()
        {
            return _descriptor.ViewModel;
        }
    }
}