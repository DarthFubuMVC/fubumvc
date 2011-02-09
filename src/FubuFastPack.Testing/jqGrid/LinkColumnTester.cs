using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using NUnit.Framework;
using System.Linq;
using FubuMVC.Tests;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class LinkColumnTester
    {
        [Test]
        public void should_return_the_display_accessor()
        {
            var column = LinkColumn<Case>.For(x => x.Condition);
            column.SelectAccessors().Select(x => x.Name).ShouldHaveTheSameElementsAs("Id", "Condition");
        }
    }
}