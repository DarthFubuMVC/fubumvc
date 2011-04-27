using System.Collections.Generic;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEngine
    {
        ISparkViewEntry GetViewEntry();
    }

    public class ViewEngine : IViewEngine
    {
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewEngine _engine;
        private readonly SparkViewDescriptor _descriptor;

        public ViewEngine(IDictionary<int, ISparkViewEntry> cache, ISparkViewEngine engine, SparkViewDescriptor descriptor)
        {
            _cache = cache;
            _engine = engine;
            _descriptor = descriptor;
        }

        public ISparkViewEntry GetViewEntry()
        {
            var entry = getEntry(_descriptor);
            return entry;
        }

        private ISparkViewEntry getEntry(SparkViewDescriptor descriptor)
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