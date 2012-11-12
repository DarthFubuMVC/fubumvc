using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class MenuItemStateTester
    {
        [Test]
        public void least_method_empty_is_available()
        {
            MenuItemState.Least().ShouldEqual(MenuItemState.Available);
        }

        [Test]
        public void least_method_with_data()
        {
            MenuItemState.Least(MenuItemState.Available).ShouldEqual(MenuItemState.Available);
            MenuItemState.Least(MenuItemState.Hidden).ShouldEqual(MenuItemState.Hidden);
            MenuItemState.Least(MenuItemState.Disabled).ShouldEqual(MenuItemState.Disabled);

            MenuItemState.Least(MenuItemState.Hidden, MenuItemState.Available).ShouldEqual(MenuItemState.Hidden);
            MenuItemState.Least(MenuItemState.Hidden, MenuItemState.Available, MenuItemState.Disabled).ShouldEqual(MenuItemState.Hidden);
            MenuItemState.Least(MenuItemState.Disabled, MenuItemState.Available).ShouldEqual(MenuItemState.Disabled);

            MenuItemState.Least(MenuItemState.Hidden, MenuItemState.Disabled).ShouldEqual(MenuItemState.Hidden);
        }

        [Test]
        public void hidden_is_not_visible_and_disabled()
        {
            MenuItemState.Hidden.IsEnabled.ShouldBeFalse();
            MenuItemState.Hidden.IsShown.ShouldBeFalse();
        }

        [Test]
        public void disabled_is_visible_but_disabled()
        {
            MenuItemState.Disabled.IsEnabled.ShouldBeFalse();
            MenuItemState.Disabled.IsShown.ShouldBeTrue();
        }

        [Test]
        public void available_is_enabled_and_shown()
        {
            MenuItemState.Available.IsEnabled.ShouldBeTrue();
            MenuItemState.Available.IsShown.ShouldBeTrue();
        }

        [Test]
        public void active_is_enabled_and_shown()
        {
            MenuItemState.Active.IsEnabled.ShouldBeTrue();
            MenuItemState.Active.IsShown.ShouldBeTrue();
        }
    }
}