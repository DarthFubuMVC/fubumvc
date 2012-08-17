using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Caching
{
    public class CachingServiceRegistry : ServiceRegistry
    {
        public CachingServiceRegistry()
        {
            SetServiceIfNone<IHeadersCache>(new HeadersCache());
            SetServiceIfNone<IOutputCache, OutputCache>();
        }
    }
}