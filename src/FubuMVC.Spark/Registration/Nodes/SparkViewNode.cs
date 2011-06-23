using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

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
            var viewDefinition = new ViewDefinition(createSparkDescriptor(true), createSparkDescriptor(false));
            
            var viewEntrySource =
                renderer.DependencyByType(typeof (IRenderAction), typeof (RenderAction))
                .DependencyByType(typeof(IViewFactory), typeof(ViewFactory))
                .DependencyByType(typeof(IViewEntrySource), typeof(ViewEntrySource));

            viewEntrySource.DependencyByValue(viewDefinition);
        }

        private SparkViewDescriptor createSparkDescriptor(bool useMaster)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(_descriptor.ViewPath);
            if (useMaster && _descriptor.Master != null)
            {
                appendMasterPage(sparkDescriptor, _descriptor.Master);
            }

            return sparkDescriptor;
        }


        private static void appendMasterPage(SparkViewDescriptor descriptor, ITemplate template)
        {
            if (template == null)
            {
                return;
            }
            descriptor.AddTemplate(template.ViewPath);
            var viewDescriptor = template.Descriptor as ViewDescriptor;
            if (viewDescriptor != null)
            {
                appendMasterPage(descriptor, viewDescriptor.Master);
            }
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