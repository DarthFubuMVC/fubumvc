using System.Diagnostics;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.TwitterBootstrap.Menus;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.TwitterBootstrap.Testing
{
    [TestFixture]
    public class MenuItemTagTester
    {
        private MenuItemToken theItem;
        private HtmlTag ul;

        [SetUp]
        public void SetUp()
        {
            theItem = new MenuItemToken{
                Key = "some key",
                Text = "some text",
                MenuItemState = MenuItemState.Available
            };
        }

        private MenuItemTag theResultingTag
        {
            get
            {
                return new MenuItemTag(theItem);
            }
        }

        private HtmlTag theLinkTag
        {
            get
            {
                var link = theResultingTag.Children.First();
                link.TagName().ShouldEqual("a");

                return link;
            }
        }

        [Test]
        public void write_the_active_class_for_the_active_item()
        {
            theItem.MenuItemState = MenuItemState.Active;

            theResultingTag.HasClass("active").ShouldBeTrue();
        }

        [Test]
        public void do_not_write_the_active_class_if_the_item_is_not_active()
        {
            theItem.MenuItemState = MenuItemState.Available;
            theResultingTag.HasClass("active").ShouldBeFalse();

            theItem.MenuItemState = MenuItemState.Disabled;
            theResultingTag.HasClass("active").ShouldBeFalse();
        }

        [Test]
        public void disable_the_link()
        {
            theItem.MenuItemState = MenuItemState.Disabled;

            theLinkTag.HasClass("disabled").ShouldBeTrue();
        }

        [Test]
        public void the_link_is_not_disabled()
        {
            theItem.MenuItemState = MenuItemState.Active;
            theLinkTag.HasClass("disabled").ShouldBeFalse();

            theItem.MenuItemState = MenuItemState.Available;
            theLinkTag.HasClass("disabled").ShouldBeFalse();
        }

        [Test]
        public void hide_the_menu_if_the_item_is_hidden()
        {
            theItem.MenuItemState = MenuItemState.Hidden;
            theResultingTag.Render().ShouldBeFalse();
        }

        [Test]
        public void do_not_hide_the_menu_if_it_is_available()
        {
            theItem.MenuItemState = MenuItemState.Active;
            theResultingTag.Render().ShouldBeTrue();

            theItem.MenuItemState = MenuItemState.Available;
            theResultingTag.Render().ShouldBeTrue();

            theItem.MenuItemState = MenuItemState.Disabled;
            theResultingTag.Render().ShouldBeTrue();
        }

        [Test]
        public void should_write_the_item_key()
        {
            Debug.WriteLine(theResultingTag);

            theLinkTag.Attr("data-key").ShouldEqual(theItem.Key);
        }

        [Test]
        public void should_write_the_link_text()
        {
            theLinkTag.FirstChild().ShouldBeOfType<LiteralTag>().Text().ShouldEqual(theItem.Text);
        }

        [Test]
        public void write_hash_if_no_url()
        {
            theItem.Url = string.Empty;

            theLinkTag.Attr("href").ShouldEqual("#");

            theItem.Url = null;

            theLinkTag.Attr("href").ShouldEqual("#");
        }

        [Test]
        public void write_url_if_it_exists_to_the_link_tag()
        {
            theItem.Url = "some url";

            theLinkTag.Attr("href").ShouldEqual(theItem.Url);
        }

        [Test]
        public void write_with_children()
        {
            theItem.Children = new MenuItemToken[]{
                new MenuItemToken{Key = "a", Text = "a-text"}, 
                new MenuItemToken{Key = "b", Text = "b-text"}, 
                new MenuItemToken{Key = "c", Text = "c-text"} 
            };

            theLinkTag.Attr("data-toggle").ShouldEqual("dropdown");
            theLinkTag.Children[1].ToString().ShouldEqual("<b class=\"caret\"></b>");

            ul = theResultingTag.Children.Last();
            ul.HasClass("dropdown-menu").ShouldBeTrue();

            ul.Children.ShouldHaveTheSameElementsAs(theItem.Children.Select(x => new MenuItemTag(x)));
        }
    }
}