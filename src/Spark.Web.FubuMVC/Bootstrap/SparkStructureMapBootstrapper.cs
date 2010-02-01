using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using Spark.Web.FubuMVC.ViewCreation;
using StructureMap;

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

            ObjectFactory.Initialize(x =>
                                         {
                                             x.For<ISparkSettings>().Use<SparkSettings>();
                                             x.For(typeof(ISparkViewRenderer<>)).Use(typeof(SparkViewRenderer<>));
                                         });

            var bootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);
            bootstrapper.Bootstrap(_routes);
        }
    }
}