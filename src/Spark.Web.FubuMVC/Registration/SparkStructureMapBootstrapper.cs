using FubuCore;
using FubuMVC.StructureMap;
using StructureMap;
using System.Web.Routing;

namespace Spark.Web.FubuMVC.Registration
{
    public class SparkStructureMapBootstrapper
    {
        private readonly RouteCollection _routes;

        private SparkStructureMapBootstrapper(RouteCollection routes)
        {
            _routes = routes;
        }

        public static void Bootstrap(RouteCollection routes, SparkFubuRegistry fubuRegistry)
        {
            new SparkStructureMapBootstrapper(routes).BootstrapStructureMap(fubuRegistry);
        }

        private void BootstrapStructureMap(SparkFubuRegistry fubuRegistry)
        {
            UrlContext.Reset();
            var bootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);
            bootstrapper.Bootstrap(_routes);
        }
    }
}