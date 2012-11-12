using System;
using System.Collections.Generic;
using FubuLocalization;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;

namespace FubuMVC.Navigation
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
        public string AddChildTo { get; set; }

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

        public IEnumerable<IMenuRegistration> ToMenuRegistrations(BehaviorChain chain)
        {
            if (AddChildTo.IsNotEmpty())
            {
                yield return new MenuRegistration(new AddChild() ,new ByName(AddChildTo), MenuNode.ForChain(Title, chain));
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