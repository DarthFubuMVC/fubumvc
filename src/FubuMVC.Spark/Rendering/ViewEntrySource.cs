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
        private readonly IViewEntryProviderCache _provider;
        private readonly SparkDescriptor _descriptor;
        public ViewEntrySource(SparkDescriptor descriptor, IViewEntryProviderCache provider)
        {
            _descriptor = descriptor;
            _provider = provider;
        }

        public ISparkViewEntry GetViewEntry()
        {
            return getViewEntry(false);
        }

        public ISparkViewEntry GetPartialViewEntry()
        {
            return getViewEntry(true);
        }

        private ISparkViewEntry getViewEntry(bool partial)
        {
            var definition = _descriptor.Definition;
            var sparkDescriptor = partial ? definition.PartialDescriptor : definition.ViewDescriptor;
            return _provider.GetViewEntry(sparkDescriptor);
        }
    }

}