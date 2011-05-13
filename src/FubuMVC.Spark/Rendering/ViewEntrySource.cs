using System.Collections.Generic;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEntrySource
    {
        ISparkViewEntry GetViewEntry();
    }

    public class ViewEntrySource : IViewEntrySource
    {
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;
        private readonly SparkViewDescriptor _descriptor;

        public ViewEntrySource(IDictionary<int, ISparkViewEntry> cache, ISparkViewEngine engine, SparkViewDescriptor descriptor)
        {
            _cache = cache;
            _engine = engine;
            _descriptor = descriptor;
        }

        public ISparkViewEntry GetViewEntry()
        {
            ISparkViewEntry entry;
            var key = _descriptor.GetHashCode();

            _cache.TryGetValue(key, out entry);
            if (entry == null || !entry.IsCurrent())
            {
                entry = _engine.CreateEntry(_descriptor);
                lock (_cache)
                {
                    _cache[key] = entry;
                }
            }
            return entry;
        }
    }
}