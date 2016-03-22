using FubuMVC.Core.Navigation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    [TestFixture]
    public class ByNameTester
    {
        [Test]
        public void matches_by_key_alone()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("something");
            matcher.Matches(key).ShouldBeTrue();
        }

        [Test]
        public void matches_by_localizer_key()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("NavigationKey:something");

            matcher.Matches(key).ShouldBeTrue();
        }

        [Test]
        public void negative_test_by_localization_key()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("OtherKey:something");

            matcher.Matches(key).ShouldBeFalse();
        }

        [Test]
        public void negative_test_by_key_alone()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("else");
            matcher.Matches(key).ShouldBeFalse();
        }

        [Test]
        public void default_key()
        {
            var matcher = new ByName("something");
            matcher.DefaultKey().ShouldBeOfType<NavigationKey>()
                .Key.ShouldBe("something");
        }
    }
}