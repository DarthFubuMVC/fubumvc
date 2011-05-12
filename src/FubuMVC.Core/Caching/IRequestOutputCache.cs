using System;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Caching
{
    public interface IRequestOutputCache<TInputModel>
    {
        void WithCache(TInputModel model, Func<TInputModel, RecordedOutput> cacheMissAction, Action<RecordedOutput> cachedDataAction);
    }

    public class RequestOutputCache<TInputModel> : IRequestOutputCache<TInputModel>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly CacheOptions<TInputModel> _options;

        public RequestOutputCache(ICacheProvider cacheProvider, CacheOptions<TInputModel> options)
        {
            _cacheProvider = cacheProvider;
            _options = options;
        }

        public void WithCache(TInputModel model, Func<TInputModel, RecordedOutput> cacheMissAction, Action<RecordedOutput> cachedDataAction)
        {
            var key = _options.KeyMaker(model);
            var result = _cacheProvider.Get(key).As<RecordedOutput>();

            if(result == null)
            {
                result = cacheMissAction(model);
                _cacheProvider.Insert(key, result, _options.Dependency, _options.AbsoluteExpiration, _options.SlidingExpiration);
            }

            cachedDataAction(result);
        }
    }
}