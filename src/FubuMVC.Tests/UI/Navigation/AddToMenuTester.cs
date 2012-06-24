using FubuMVC.Core.UI.Navigation;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;
using System.Linq;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class AddToMenuTester
    {
        [Test]
        public void format_description()
        {
            var key = new NavigationKey("something");
            new AddToMenu().FormatDescription("parent", key)
                .ShouldEqual("Add '{0}' to menu 'parent'".ToFormat(key.ToLocalizationKey()));
        }

        [Test]
        public void apply()
        {
            var dependency = new MenuChain("something");
            var node = MenuNode.Node("else");

            new AddToMenu().Apply(dependency, node);

            dependency.Top.ShouldBeTheSameAs(node);
        }
    }
}