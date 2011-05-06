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

        /// <summary>
        /// Cache the ouput of this model by session
        /// </summary>
        /// <returns></returns>
        public static CacheOptions<TModel> BySession()
        {
            return new CacheOptions<TModel>(m=>HttpContext.Current.Session.SessionID);
        }
    }
}