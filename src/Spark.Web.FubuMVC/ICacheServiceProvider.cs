using System.Web;
using Spark.Caching;

namespace Spark.Web.FubuMVC
{
    public interface ICacheServiceProvider
    {
        ICacheService GetCacheService();
    }

    public class DefaultCacheServiceProvider : ICacheServiceProvider
    {
        public ICacheService GetCacheService()
        {
            return HttpRuntime.Cache != null ? new DefaultCacheService(HttpRuntime.Cache) : null;
        }
    }
}