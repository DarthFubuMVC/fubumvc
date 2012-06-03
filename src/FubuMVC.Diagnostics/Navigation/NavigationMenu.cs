using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Diagnostics.Navigation
{
    [MarkedForTermination]
    public class NavigationMenu
    {
    	public NavigationMenu()
    	{
    		Items = new List<NavigationMenuItem>();
    	}

        public IEnumerable<NavigationMenuItem> Items { get; set; }
    }
}