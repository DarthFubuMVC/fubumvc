using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using StructureMap;

namespace FubuMVC.StructureMap.Bootstrap
{
    public class FubuStructureMapBootstrapper
    {
        private readonly RouteCollection _routes;

        private FubuStructureMapBootstrapper(RouteCollection routes)
        {
            _routes = routes;
        }

        public static void Bootstrap(RouteCollection routes, FubuRegistry fubuRegistry)
        {
            new FubuStructureMapBootstrapper(routes).BootstrapStructureMap(fubuRegistry);
        }

        private void BootstrapStructureMap(FubuRegistry fubuRegistry)
        {
            UrlContext.Reset();

            ObjectFactory.Initialize(x => { });

            var fubuBootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);

            fubuBootstrapper.Bootstrap(_routes);
        }
    }
}