using System.Collections.Generic;
using System.Linq;
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
        private readonly IEnumerable<ISparkViewModification> _modifications;
        private readonly ISparkViewEngine _engine;

        public SparkViewProvider(IDictionary<int, ISparkViewEntry> cache, SparkViewDescriptor descriptor, IEnumerable<ISparkViewModification> modifications, ISparkViewEngine engine)
        {
            _descriptor = descriptor;
            _modifications = modifications;
            _cache = cache;
            _engine = engine;
        }

        public ISparkView GetView()
        {
            var viewEntry = getViewEntry();
            var view = viewEntry.CreateInstance();
            
            _modifications
                .Where(m => m.Applies(view))
                .Each(m => m.Modify(view));

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