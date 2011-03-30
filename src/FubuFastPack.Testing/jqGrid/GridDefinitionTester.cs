using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class GridDefinitionTester
    {
    
        [Test]
        public void if_no_explicit_sort_use_the_first_column()
        {
            var definition = new GridDefinition<Case>();
            definition.Show(x => x.Condition);

            definition.SortBy.FieldName.ShouldEqual("Condition");
            definition.SortBy.IsAscending.ShouldBeTrue();
        }

        [Test]
        public void if_no_explicit_sort_use_the_first_column_if_the_first_column_is_a_view_link()
        {
            var definition = new GridDefinition<Case>();
            definition.ShowViewLink(x => x.Condition);

            definition.SortBy.FieldName.ShouldEqual("Condition");
            definition.SortBy.IsAscending.ShouldBeTrue();
        }

        [Test]
        public void explicit_default_sort()
        {
            var definition = new GridDefinition<Case>();
            definition.ShowViewLink(x => x.Condition);
            definition.SortBy = SortRule<Case>.Descending(x => x.Created);

            definition.SortBy.FieldName.ShouldEqual("Created");
            definition.SortBy.IsAscending.ShouldBeFalse();
        }
    }

    
}