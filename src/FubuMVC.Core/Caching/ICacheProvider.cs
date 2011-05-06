using System;
using System.Web.Caching;

namespace FubuMVC.Core.Caching
{
    public interface ICacheProvider
    {
        object Get(string key);

        void Insert(string key, object value, CacheDependency cacheDependency, DateTime absoluteExpiration,TimeSpan slidingExpiration);
    }
}