using System.Collections.Generic;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    

    public interface IContextualMenu<T>
    {
        IEnumerable<MenuItemToken> MenuItemsFor(T target);
        IEnumerable<MenuItemToken> MenuItemsFor(T target, string category);
    }

    public static class ContextualMenuExtensions
    {
        public static IEnumerable<MenuItemToken> MenuItemsFor<T>(this IContextualMenu<T> menu, T target,
                                                                 StringToken name)
        {
            return menu.MenuItemsFor(target, name.Key);
        }
    }

}