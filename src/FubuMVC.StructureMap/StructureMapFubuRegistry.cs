using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap
{
    public class StructureMapFubuRegistry : Registry
    {
        public StructureMapFubuRegistry()
        {
            For<HttpRequestWrapper>().Use(c => BuildRequestWrapper());


            For<HttpContextBase>().Use<HttpContextWrapper>().Ctor<HttpContext>().Is(
                x => x.ConstructedBy(BuildContextWrapper));
            For<IServiceLocator>().Use<StructureMapServiceLocator>();

            For<ISessionState>().Use<SimpleSessionState>();

            For<CurrentRequest>().Use(c => c.GetInstance<IFubuRequest>().Get<CurrentRequest>());
        }

        public HttpContext BuildContextWrapper()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current;
                }
            }
            catch (HttpException)
            {
                //This is only here for web startup when HttpContext.Current is not available.
            }

            return null;
        }

        public static HttpRequestWrapper BuildRequestWrapper()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    return new HttpRequestWrapper(HttpContext.Current.Request);
                }
            }
            catch (HttpException)
            {
                //This is only here for web startup when HttpContext.Current is not available.
            }

            return null;
        }
    }
}