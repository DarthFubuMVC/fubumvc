using System.Collections.Generic;
using FubuMVC.Razor.RazorEngine;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewEntryProviderCache
    {
        IRazorViewEntry GetViewEntry(RazorViewDescriptor descriptor);
    }

    public class ViewEntryProviderCache : IViewEntryProviderCache
    {
        private readonly IDictionary<int, IRazorViewEntry> _cache;
        private readonly IRazorViewEngine _engine;

        public ViewEntryProviderCache(IRazorViewEngine engine)
        {
            _cache = new Dictionary<int, IRazorViewEntry>();
            _engine = engine;
        }

        public IRazorViewEntry GetViewEntry(RazorViewDescriptor descriptor)
        {
            IRazorViewEntry entry;
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