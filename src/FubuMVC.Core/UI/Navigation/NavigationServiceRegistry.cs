using Bottles;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationServiceRegistry : ServiceRegistry
    {
        public NavigationServiceRegistry()
        {
            AddService<IActivator, NavigationActivator>();
            SetServiceIfNone<INavigationService, NavigationService>();
            SetServiceIfNone<IMenuStateService, MenuStateService>();
        }
    }
}