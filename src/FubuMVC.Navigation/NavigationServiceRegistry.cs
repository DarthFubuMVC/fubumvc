using FubuMVC.Core.Registration;

namespace FubuMVC.Navigation
{
    public class NavigationServiceRegistry : ServiceRegistry
    {
        public NavigationServiceRegistry()
        {
            SetServiceIfNone<INavigationService, NavigationService>();
            SetServiceIfNone<IMenuStateService, MenuStateService>();
        }
    }
}