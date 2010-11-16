using System.Collections.Generic;

namespace FubuMVC.Core.UI.Navigation
{
    

    public interface IContextualMenu<T>
    {
        IEnumerable<MenuItemToken> MenuItemsFor(T target);
        IEnumerable<MenuItemToken> MenuItemsFor(T target, string category);
    }

}