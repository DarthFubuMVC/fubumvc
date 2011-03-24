using FubuMVC.Diagnostics.Navigation;

namespace FubuMVC.Diagnostics.Partials
{
    public class NavigationMenuDecorator : IPartialDecorator<NavigationMenu>
    {
        private readonly INavigationMenuBuilder _builder;

        public NavigationMenuDecorator(INavigationMenuBuilder builder)
        {
            _builder = builder;
        }

        public NavigationMenu Enrich(NavigationMenu target)
        {
            target.Items = _builder.MenuItems();
            return target;
        }
    }
}