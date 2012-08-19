using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Navigation;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class NavigationServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
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