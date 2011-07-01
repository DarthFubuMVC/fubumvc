using System.Collections.Generic;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEntryProviderCache
    {
        ISparkViewEntry GetViewEntry(SparkViewDescriptor descriptor);
    }

    public class ViewEntryProviderCache : IViewEntryProviderCache
    {
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;

        public ViewEntryProviderCache(ISparkViewEngine engine)
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
            _engine = engine;
        }

        public ISparkViewEntry GetViewEntry(SparkViewDescriptor descriptor)
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