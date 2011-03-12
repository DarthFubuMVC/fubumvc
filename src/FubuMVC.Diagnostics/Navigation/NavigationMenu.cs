using System.Collections.Generic;
using FubuMVC.Diagnostics.Configuration.Partials;

namespace FubuMVC.Diagnostics.Navigation
{
    public class NavigationMenu : IPartialModel
    {
        public IEnumerable<NavigationMenuItem> Items { get; set; }
    }
}