using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Navigation
{
    public interface INavigationMenuBuilder
    {
        IEnumerable<NavigationMenuItem> MenuItems();
    }
}