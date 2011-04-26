using System.Collections.Generic;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkViewProvider
    {
        ISparkView GetView();
    }

    public class SparkViewProvider : ISparkViewProvider
    {
        private readonly SparkViewDescriptor _descriptor;
        private readonly IDictionary<int, ISparkViewEntry> _cache;
        private readonly ISparkViewActivator _activator;
        private readonly ISparkViewEngine _engine;

        public SparkViewProvider(IDictionary<int, ISparkViewEntry> cache, SparkViewDescriptor descriptor, ISparkViewActivator activator, ISparkViewEngine engine)
        {
            _descriptor = descriptor;
            _cache = cache;
            _activator = activator;
            _engine = engine;
        }

        public ISparkView GetView()
        {
            var viewEntry = getViewEntry();
            var view = viewEntry.CreateInstance();
            _activator.Activate(view);

            return view;
        }

        private ISparkViewEntry getViewEntry()
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