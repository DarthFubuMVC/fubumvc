using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;
using System.Web.Routing;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class SparkStructureMapBootstrapper
    {
        private readonly RouteCollection _routes;

        private SparkStructureMapBootstrapper(RouteCollection routes)
        {
            _routes = routes;
        }

        public static void Bootstrap(RouteCollection routes, FubuRegistry fubuRegistry)
        {
            new SparkStructureMapBootstrapper(routes).BootstrapStructureMap(fubuRegistry);
        }

        private void BootstrapStructureMap(FubuRegistry fubuRegistry)
        {
            UrlContext.Reset();
            var bootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);
            bootstrapper.Bootstrap(_routes);
        }
    }
}