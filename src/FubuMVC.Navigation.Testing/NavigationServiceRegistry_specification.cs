using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class NavigationServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildFrom(x => x.Import<NavigationRegistryExtension>()).Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
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
    }
}