using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.UI.Navigation;
using HtmlTags;

namespace FubuMVC.TwitterBootstrap.Menus
{
    public class MenuItemTag : HtmlTag
    {
        private readonly MenuItemToken _item;

        public MenuItemTag(MenuItemToken item) : base("li")
        {
            _item = item;

            if (item.MenuItemState == MenuItemState.Hidden)
            {
                Render(false);
                return;
            }

            var link = buildLink(item);

            if (item.Children.Any())
            {
                writeChildren(item, link);
            }

            setActiveState(item);
            setDisabledState(item, link);
        }

        private static void setDisabledState(MenuItemToken item, HtmlTag link)
        {
            if (item.MenuItemState == MenuItemState.Disabled)
            {
                link.AddClass("disabled");
            }
        }

        private void setActiveState(MenuItemToken item)
        {
            if (item.MenuItemState == MenuItemState.Active)
            {
                AddClass("active");
            }
        }

        private HtmlTag buildLink(MenuItemToken item)
        {
            var link = Add("a").Append(new LiteralTag(item.Text));
            link.Data("key", item.Key);

            string linkUrl = item.Url.IsNotEmpty() ? item.Url : "#";
            link.Attr("href", linkUrl);
            return link;
        }

        private void writeChildren(MenuItemToken item, HtmlTag link)
        {
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

        public bool Equals(MenuItemTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._item, _item);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MenuItemTag)) return false;
            return Equals((MenuItemTag) obj);
        }

        public override int GetHashCode()
        {
            return (_item != null ? _item.GetHashCode() : 0);
        }
    }
}