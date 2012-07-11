using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.UI.Navigation;
using HtmlTags;

namespace FubuMVC.TwitterBootstrap.Menus
{
    public class MenuItemTag : HtmlTag
    {
        public MenuItemTag(MenuItemToken item) : base("li")
        {
            var link = Add("a").Append(new LiteralTag(item.Text));
            if (item.Url.IsNotEmpty())
            {
                link.Attr("href", item.Url);
            }

            if (item.Children.Any())
            {
                link.Attr("href", "#");
                link.AddClass("dropdown-toggle");
                link.Attr("data-toggle", "dropdown");

                link.Add("b").AddClass("caret");

                var ul = Add("ul").AddClass("dropdown-menu");
                item.Children.Each(child =>
                {
                    var childTag = new MenuItemTag(child);
                    ul.Append(childTag);
                });
            }

            if (item.MenuItemState == MenuItemState.Active)
            {
                AddClass("active");
            }
        }
    }
}