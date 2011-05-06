using System;
using System.Web;
using System.Web.Caching;

namespace FubuMVC.Core.Caching
{
    public class AspNetRuntimeCacheProvider : ICacheProvider
    {
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Insert(string key, object value, CacheDependency cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, cacheDependency,absoluteExpiration, slidingExpiration );
        }
    }
}