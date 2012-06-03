using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public interface INavigationMenuBuilder
    {
        IEnumerable<NavigationMenuItem> MenuItems();
    }
}