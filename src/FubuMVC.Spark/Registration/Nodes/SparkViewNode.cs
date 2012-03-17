using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewNode : OutputNode<RenderViewBehavior>, IMayHaveInputType
    {
        private readonly SparkDescriptor _descriptor;

        public SparkViewNode(SparkDescriptor descriptor)
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

        public override string ToString()
        {
            return Description;
        }
    }
}