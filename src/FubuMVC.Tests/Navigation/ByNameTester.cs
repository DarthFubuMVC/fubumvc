using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class ByNameTester
    {
        [Fact]
        public void matches_by_key_alone()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("something");
            matcher.Matches(key).ShouldBeTrue();
        }

        [Fact]
        public void matches_by_localizer_key()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("NavigationKey:something");

            matcher.Matches(key).ShouldBeTrue();
        }

        [Fact]
        public void negative_test_by_localization_key()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("OtherKey:something");

            matcher.Matches(key).ShouldBeFalse();
        }

        [Fact]
        public void negative_test_by_key_alone()
        {
            var key = new NavigationKey("something");

            var matcher = new ByName("else");
            matcher.Matches(key).ShouldBeFalse();
        }

        [Fact]
        public void default_key()
        {
            var matcher = new ByName("something");
            matcher.DefaultKey().ShouldBeOfType<NavigationKey>()
                .Key.ShouldBe("something");
        }
    }
}