using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Navigation
{
    public class NavigationServiceRegistry : ServiceRegistry
    {
        public NavigationServiceRegistry()
        {
            SetServiceIfNone<INavigationService, NavigationService>();
            SetServiceIfNone<IMenuStateService, MenuStateService>();
            SetServiceIfNone<IMenuResolver, MenuResolverCache>();
        }
    }
}