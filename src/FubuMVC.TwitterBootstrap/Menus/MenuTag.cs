using System.Collections.Generic;
using FubuMVC.Core.UI.Navigation;
using HtmlTags;

namespace FubuMVC.TwitterBootstrap.Menus
{
    public class MenuTag : HtmlTag
    {
        public MenuTag(IEnumerable<MenuItemToken> tokens) : base("ul")
        {
            AddClass("nav");
            tokens.Each(token => Append(new MenuItemTag(token)));
        }
    }
}