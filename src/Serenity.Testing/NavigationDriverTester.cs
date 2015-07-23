using NUnit.Framework;
using Shouldly;

namespace Serenity.Testing
{
    [TestFixture]
    public class NavigationDriverTester
    {
        [Test]
        public void by_default_uses_a_nullo_after_navigation()
        {
            var driver = new NavigationDriver(null);
            driver.AfterNavigation.ShouldBeOfType<NulloAfterNavigation>();
        }
    }
}