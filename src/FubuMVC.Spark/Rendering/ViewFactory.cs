using System;
using FubuCore.Descriptions;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public class ViewFactory
    {
        private readonly SparkDescriptor _descriptor;

        public ViewFactory(SparkDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public IRenderableView GetView()
        {
            return getView(_descriptor.ViewEntry);
        }

        public IRenderableView GetPartialView()
        {
            return getView(_descriptor.ViewEntry);
        }

        private IFubuSparkView getView(ISparkViewEntry entry)
        {
            var view = (IFubuSparkView)entry.CreateInstance();
            return view;
        }

        public void Describe(Description description)
        {
            description.Title = "Spark View " + _descriptor.ViewEntry.ViewId;
        }
    }
}