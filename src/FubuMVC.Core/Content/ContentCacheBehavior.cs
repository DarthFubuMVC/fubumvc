using System;
using System.Web;

namespace FubuMVC.Core.Content
{
    public class ContentCacheBehavior : IContentCacheBehavior
    {
        private Func<string, Action<HttpCachePolicy>> _cachingAction;

        public ContentCacheBehavior()
        {
            //no caching by default
            _cachingAction = filename => context => { };
        }

        public void RegisterCacheBehavior(Func<string, Action<HttpCachePolicy>> cachingAction)
        {
            _cachingAction = cachingAction;
        }

        public void ApplyCacheBehavior(string filename, HttpCachePolicy cache)
        {
            var cacheAction = _cachingAction(filename);
            cacheAction(cache);
        }
    }
}