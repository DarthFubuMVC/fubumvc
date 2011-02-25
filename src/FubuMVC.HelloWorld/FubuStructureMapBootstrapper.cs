using System.Web.Routing;
using FubuCore;
using FubuMVC.HelloWorld.Services;
using FubuMVC.StructureMap;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace FubuMVC.HelloWorld
{
    public class FubuStructureMapBootstrapper : IBootstrapper
    {
        private readonly RouteCollection _routes;

        private FubuStructureMapBootstrapper(RouteCollection routes)
        {
            _routes = routes;
        }

        public void BootstrapStructureMap()
        {
            UrlContext.Reset();
            ObjectFactory.Initialize(x => x.For<IHttpSession>().Use<CurrentHttpContextSession>());
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));
            BootstrapFubu(ObjectFactory.Container, _routes);
        }

        private static void BootstrapFubu(IContainer container, RouteCollection routes)
        {
            var bootstrapper = new StructureMapBootstrapper(container, new HelloWorldFubuRegistry());
            bootstrapper.Bootstrap(routes);
        }

        public static void Bootstrap(RouteCollection routes)
        {
            new FubuStructureMapBootstrapper(routes).BootstrapStructureMap();
        }
    }
}