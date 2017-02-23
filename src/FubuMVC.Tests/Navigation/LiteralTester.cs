using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class LiteralTester
    {
        [Fact]
        public void matches_positive()
        {
            var key = new NavigationKey("something");

            new Literal(key).Matches(key).ShouldBeTrue();
        }

        [Fact]
        public void matches_negative()
        {
            var key = new NavigationKey("something");
            new Literal(new NavigationKey("else")).Matches(key).ShouldBeFalse();
        }

        [Fact]
        public void default_key()
        {
            var key = new NavigationKey("something");
            new Literal(key).DefaultKey().ShouldBeTheSameAs(key);
        }
    }
}