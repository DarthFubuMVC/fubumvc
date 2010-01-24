using System.Web.Routing;
using Spark.Caching;

namespace Spark.Web.FubuMVC
{
    public interface ICacheServiceProvider
    {
        ICacheService GetCacheService(RequestContext requestContext);
    }

    public class DefaultCacheServiceProvider : ICacheServiceProvider
    {
        public ICacheService GetCacheService(RequestContext context)
        {
            if (context.HttpContext != null && context.HttpContext.Cache != null)
                return new DefaultCacheService(context.HttpContext.Cache);
            return null;
        }
    }

}