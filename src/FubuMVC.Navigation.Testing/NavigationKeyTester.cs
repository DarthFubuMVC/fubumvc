using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class NavigationKeyTester
    {
        [Test]
        public void equals_works()
        {
            new NavigationKey("one").ShouldEqual(new NavigationKey("one"));
            new NavigationKey("two").ShouldEqual(new NavigationKey("two"));
            new NavigationKey("one").ShouldNotEqual(new NavigationKey("two"));
        }

        [Test]
        public void get_hash_code_is_predictable()
        {
            new NavigationKey("one").GetHashCode().ShouldEqual(new NavigationKey("one").GetHashCode());
            new NavigationKey("two").GetHashCode().ShouldEqual(new NavigationKey("two").GetHashCode());
            new NavigationKey("one").GetHashCode().ShouldNotEqual(new NavigationKey("two").GetHashCode());
        }
    }
}