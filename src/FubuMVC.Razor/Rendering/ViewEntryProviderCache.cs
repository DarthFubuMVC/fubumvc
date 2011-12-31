using System.Collections.Generic;
using FubuMVC.Razor.RazorEngine;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewEntryProviderCache
    {
        IRazorViewEntry GetViewEntry(ViewDescriptor descriptor);
    }

    public class ViewEntryProviderCache : IViewEntryProviderCache
    {
        private readonly IDictionary<int, IRazorViewEntry> _cache;
        private readonly IRazorViewEntryFactory _entryFactory;

        public ViewEntryProviderCache(IRazorViewEntryFactory entryFactory)
        {
            _cache = new Dictionary<int, IRazorViewEntry>();
            _entryFactory = entryFactory;
        }

        public IRazorViewEntry GetViewEntry(ViewDescriptor descriptor)
        {
            IRazorViewEntry entry;
            var key = descriptor.GetHashCode();

            _cache.TryGetValue(key, out entry);
            if (entry == null || !entry.IsCurrent())
            {
                entry = _entryFactory.CreateEntry(descriptor);
                lock (_cache)
                {
                    _cache[key] = entry;
                }
            }
            return entry;
        }
    }
}