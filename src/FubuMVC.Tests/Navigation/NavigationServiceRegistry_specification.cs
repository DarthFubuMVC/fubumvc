using FubuMVC.Core;
using FubuMVC.Core.Navigation;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Navigation
{
    [TestFixture]
    public class NavigationServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            var registry = new FubuRegistry();
            registry.Import<NavigationRegistryExtension>();

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                container.Model.For<TService>().Default.ReturnedType.ShouldBe(typeof(TImplementation));
            }
        }


        [Test]
        public void menu_state_service_is_registered()
        {
            registeredTypeIs<IMenuStateService, MenuStateService>();
        }

        [Test]
        public void navigation_service_is_registered()
        {
            registeredTypeIs<INavigationService, NavigationService>();
        }

        [Test]
        public void menu_resolver()
        {
            registeredTypeIs<IMenuResolver, MenuResolverCache>();

            ServiceRegistry.ShouldBeSingleton(typeof(MenuResolverCache))
                .ShouldBeTrue();
        }
    }
}