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
        private readonly IViewDefinitionResolver _resolver;
        public ViewEntrySource(SparkDescriptor descriptor, IViewEntryProviderCache provider, IViewDefinitionResolver resolver)
        {
            _descriptor = descriptor;
            _provider = provider;
            _resolver = resolver;
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
            var definition = _resolver.Resolve(_descriptor);
            var sparkDescriptor = partial ? definition.PartialDescriptor : definition.ViewDescriptor;
            return _provider.GetViewEntry(sparkDescriptor);
        }
    }

}