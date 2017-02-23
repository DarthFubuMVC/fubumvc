using FubuCore;
using FubuMVC.Core.Navigation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class AddToMenuTester
    {
        [Fact]
        public void format_description()
        {
            var key = new NavigationKey("something");
            new AddChild().FormatDescription("parent", key)
                .ShouldBe("Add '{0}' to menu 'parent'".ToFormat(key.ToLocalizationKey()));
        }

        [Fact]
        public void apply()
        {
            var dependency = new MenuChain("something");
            var node = MenuNode.Node("else");

            new AddChild().Apply(dependency, node);

            dependency.Top.ShouldBeTheSameAs(node);
        }
    }
}