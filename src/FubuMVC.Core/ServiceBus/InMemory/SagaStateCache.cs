using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    public class SagaStateCache<T> : ISagaStateCache<T> where T : class
    {
        private readonly IDictionary<Guid, T> _cache = new ConcurrentDictionary<Guid, T>();

        public void Store(Guid correlationId, T state)
        {
            _cache[correlationId] = state;
        }

        public T Find(Guid correlationId)
        {
            T state;
            return _cache.TryGetValue(correlationId, out state) ? state : null;
        }

        public void Delete(Guid correlationId)
        {
            _cache.Remove(correlationId);
        }
    }
}