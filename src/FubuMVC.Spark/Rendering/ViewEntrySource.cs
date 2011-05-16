using System.Collections.Generic;
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
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;
        private readonly ViewDefinition _viewDefinition;

        public ViewEntrySource(ISparkViewEngine engine, ViewDefinition viewDefinition, IDictionary<int, ISparkViewEntry> cache)
        {
            _cache = cache;
            _engine = engine;
            _viewDefinition = viewDefinition;
        }

        public ISparkViewEntry GetViewEntry()
        {
            return getViewEntry(_viewDefinition.ViewDescriptor);
        }

        public ISparkViewEntry GetPartialViewEntry()
        {
            return getViewEntry(_viewDefinition.PartialDescriptor);
        }

        private ISparkViewEntry getViewEntry(SparkViewDescriptor descriptor)
        {
            ISparkViewEntry entry;
            var key = descriptor.GetHashCode();

            _cache.TryGetValue(key, out entry);
            if (entry == null || !entry.IsCurrent())
            {
                entry = _engine.CreateEntry(descriptor);
                lock (_cache)
                {
                    _cache[key] = entry;
                }
            }
            return entry;
        }

    }


}