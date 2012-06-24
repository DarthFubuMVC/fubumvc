using FubuMVC.Core.UI.Navigation;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class LiteralTester
    {
        [Test]
        public void matches_positive()
        {
            var key = new NavigationKey("something");

            new Literal(key).Matches(key).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            var key = new NavigationKey("something");
            new Literal(new NavigationKey("else")).Matches(key).ShouldBeFalse();
        }

        [Test]
        public void default_key()
        {
            var key = new NavigationKey("something");
            new Literal(key).DefaultKey().ShouldBeTheSameAs(key);
        }
    }
}