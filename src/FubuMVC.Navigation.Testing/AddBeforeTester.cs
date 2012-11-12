using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class AddBeforeTester
    {
        [Test]
        public void format_description()
        {
            var key = new NavigationKey("something");
            new AddBefore().FormatDescription("parent", key)
                .ShouldEqual("Insert '{0}' before 'parent'".ToFormat(key.ToLocalizationKey()));
        }

        [Test]
        public void apply()
        {
            var dependency = MenuNode.Node("something");
            dependency.AddBefore(MenuNode.Node("third"));
            var node = MenuNode.Node("else");

            new AddBefore().Apply(dependency, node);

            dependency.Previous.ShouldBeTheSameAs(node);
        }
    }
}