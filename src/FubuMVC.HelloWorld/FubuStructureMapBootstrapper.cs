using System.Web.Routing;
using FubuCore;
using FubuMVC.HelloWorld.Services;
using FubuMVC.StructureMap;
using FubuValidation;
using FubuValidation.Registration;
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
            Validator.Initialize<HelloWorldValidationRegistry>();
            ObjectFactory.Initialize(x =>
                                        {
                                            x.For<IHttpSession>().Use<CurrentHttpContextSession>();
                                            x.For<IValidationProvider>().Use(() => Validator.Provider);
                                            x.For<IValidationQuery>().Use(() => Validator.Model);
                                        });
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