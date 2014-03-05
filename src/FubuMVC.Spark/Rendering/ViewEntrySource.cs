using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEntrySource
    {
        ISparkViewEntry GetViewEntry();
        ISparkViewEntry GetPartialViewEntry();
    }

    public class ViewEntrySource : IViewEntrySource
    {
        private readonly SparkDescriptor _descriptor;
        public ViewEntrySource(SparkDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public ISparkViewEntry GetViewEntry()
        {
            return _descriptor.ViewEntry;
        }

        public ISparkViewEntry GetPartialViewEntry()
        {
            return _descriptor.PartialViewEntry;
        }
    }

}