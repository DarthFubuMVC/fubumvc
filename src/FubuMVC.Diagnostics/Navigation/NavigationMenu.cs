using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Navigation
{
    public class NavigationMenu
    {
    	public NavigationMenu()
    	{
    		Items = new List<NavigationMenuItem>();
    	}

        public IEnumerable<NavigationMenuItem> Items { get; set; }
    }
}