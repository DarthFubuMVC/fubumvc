using System;
using System.Collections.Generic;
using FubuCore;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.UI.Navigation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuItemAttribute : Attribute
    {
        private readonly StringToken _title;

        public MenuItemAttribute(string title)
        {
            _title = new NavigationKey(title);
        }

        public MenuItemAttribute(string key, string defaultText)
        {
            _title = new NavigationKey(key, defaultText);
        }

        /// <summary>
        /// Add this menu item to another menu item or node
        /// </summary>
        /// <example>"Log Out" or "MyNavigationKey:LogOut" if you need to refer to a StringToken</example>
        public string AddToMenu { get; set; }

        /// <summary>
        /// Add this menu item after another menu item or node
        /// </summary>
        /// <example>"Log Out" or "MyNavigationKey:LogOut" if you need to refer to a StringToken</example>
        public string AddAfter { get; set; }

        /// <summary>
        /// Add this menu item before another menu item or node
        /// </summary>
        /// <example>"Log Out" or "MyNavigationKey:LogOut" if you need to refer to a StringToken</example>
        public string AddBefore { get; set; }

        public IEnumerable<IMenuRegistration> ToMenuRegistration(BehaviorChain chain)
        {
            if (AddToMenu.IsNotEmpty())
            {
                yield return new MenuRegistration(new AddToMenu() ,new ByName(AddToMenu), MenuNode.ForChain(Title, chain));
            }

            if (AddAfter.IsNotEmpty())
            {
                yield return new MenuRegistration(new AddAfter(),new ByName(AddAfter), MenuNode.ForChain(Title, chain));
            }

            if (AddBefore.IsNotEmpty())
            {
                yield return new MenuRegistration(new AddBefore(),new ByName(AddBefore), MenuNode.ForChain(Title, chain));
            }
        }

        public StringToken Title
        {
            get { return _title; }
        }
    }
}