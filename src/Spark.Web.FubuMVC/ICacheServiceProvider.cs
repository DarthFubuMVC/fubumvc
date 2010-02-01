using System.Web;
using Spark.Caching;

namespace Spark.Web.FubuMVC
{
    public interface ICacheServiceProvider
    {
        ICacheService GetCacheService(HttpContextBase httpContext);
    }

    public class DefaultCacheServiceProvider : ICacheServiceProvider
    {
        #region ICacheServiceProvider Members

        public ICacheService GetCacheService(HttpContextBase httpContext)
        {
            if (httpContext != null && httpContext.Cache != null)
                return new DefaultCacheService(httpContext.Cache);
            return null;
        }

        #endregion
    }
}