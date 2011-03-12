using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Diagnostics.Navigation
{
    public interface INavigationMenuBuilder
    {
        IEnumerable<NavigationMenuItem> MenuItems();
    }

    public class NavigationMenuBuilder : INavigationMenuBuilder
    {
        private readonly IEnumerable<INavigationItemAction> _actions;

        public NavigationMenuBuilder(IEnumerable<INavigationItemAction> actions)
        {
            _actions = actions;
        }

        public IEnumerable<NavigationMenuItem> MenuItems()
        {
            return _actions.Select(a => new NavigationMenuItem
                                            {
                                                Text = a.Text(),
                                                Url = a.Url()
                                            });
        }
    }
}