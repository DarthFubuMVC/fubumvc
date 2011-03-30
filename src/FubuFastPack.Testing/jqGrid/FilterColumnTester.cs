using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class FilterColumnTester
    {
        private FilterColumn<Case> theColumn;

        [SetUp]
        public void SetUp()
        {
            theColumn = new FilterColumn<Case>(c => c.Condition);
        }

        [Test]
        public void to_dictionary_returns_empty_enumeration()
        {
            theColumn.ToDictionary().Any().ShouldBeFalse();
        }

        [Test]
        public void headers_returns_empty_enumeration()
        {
            theColumn.Headers().Any().ShouldBeFalse();
        }

        [Test]
        public void selected_accessors_returns_empty_enumeration()
        {
            theColumn.SelectAccessors().Any().ShouldBeFalse();
        }
    }
}