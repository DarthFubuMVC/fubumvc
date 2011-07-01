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
        private readonly ViewDefinition _viewDefinition;
        private readonly ISparkDescriptorResolver _resolver;
        public ViewEntrySource(ViewDefinition viewDefinition, IViewEntryProviderCache provider, ISparkDescriptorResolver resolver)
        {
            _provider = provider;
            _resolver = resolver;
            _viewDefinition = viewDefinition;
        }
        public ISparkViewEntry GetViewEntry()
        {
            var descriptor = _resolver.ResolveDescriptor(_viewDefinition);
            return _provider.GetViewEntry(descriptor);
        }
        public ISparkViewEntry GetPartialViewEntry()
        {
            var descriptor = _resolver.ResolvePartialDescriptor(_viewDefinition);
            return _provider.GetViewEntry(descriptor);
        }
    }
}