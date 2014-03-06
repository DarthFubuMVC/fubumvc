using System;
using FubuCore;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewToken : IViewToken
    {
        private readonly SparkDescriptor _descriptor;

        public SparkViewToken(SparkDescriptor viewDescriptor)
        {
            _descriptor = viewDescriptor;
        }

        public IRenderableView GetView()
        {
            return _descriptor.ViewEntry.CreateInstance().As<IRenderableView>();
        }

        public IRenderableView GetPartialView()
        {
            return _descriptor.PartialViewEntry.CreateInstance().As<IRenderableView>();
        }

        public string ProfileName { get; set; }

        public Type ViewType
        {
            get { return typeof (ISparkView); }
        }

        public Type ViewModel
        {
            get { return _descriptor.Template.ViewModel; }
        }

        public string Name()
        {
            return _descriptor.Template.Name();
        }

        public string Namespace
        {
            get { return _descriptor.Template.Namespace; }
        }

        public override string ToString()
        {
            return _descriptor.Template.RelativePath();
        }
    }
}