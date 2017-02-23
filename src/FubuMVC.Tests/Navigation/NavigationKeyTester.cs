using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class NavigationKeyTester
    {
        [Fact]
        public void equals_works()
        {
            new NavigationKey("one").ShouldBe(new NavigationKey("one"));
            new NavigationKey("two").ShouldBe(new NavigationKey("two"));
            new NavigationKey("one").ShouldNotBe(new NavigationKey("two"));
        }

        [Fact]
        public void get_hash_code_is_predictable()
        {
            new NavigationKey("one").GetHashCode().ShouldBe(new NavigationKey("one").GetHashCode());
            new NavigationKey("two").GetHashCode().ShouldBe(new NavigationKey("two").GetHashCode());
            new NavigationKey("one").GetHashCode().ShouldNotBe(new NavigationKey("two").GetHashCode());
        }
    }
}