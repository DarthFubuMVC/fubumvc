using System;
using System.Web;
using System.Web.Caching;

namespace FubuMVC.Core.Caching
{
    public class CacheOptions<TModel>
    {
        public CacheOptions(Func<TModel, string> keyMaker)
        {
            KeyMaker = keyMaker;
            Dependency = null;
            AbsoluteExpiration = DateTime.MaxValue;
            SlidingExpiration = new TimeSpan(0,0,10,0);
        }

        public Func<TModel, string> KeyMaker { get; set; }
        public CacheDependency Dependency { get; set; }
        public DateTime AbsoluteExpiration { get; set; }
        public TimeSpan SlidingExpiration { get; set; }
    }
}