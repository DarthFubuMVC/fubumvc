using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuFastPack.Testing.Security;
using FubuLocalization;
using FubuMVC.Tests;
using NUnit.Framework;
using System.Linq;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class GridColumnTester
    {
        private GridColumn<Case> theColumn;
        private QueryService queryService;

        [SetUp]
        public void SetUp()
        {
            theColumn = GridColumn<Case>.ColumnFor(x => x.Condition);
            queryService = new QueryService(new IFilterHandler[0]);
        }


        [Test]
        public void get_header_with_a_string_token()
        {
            StringToken token = StringToken.FromKeyString("HeaderText");
            theColumn.OverrideHeader(token);

            theColumn.GetHeader().ShouldEqual(token.ToString());
        }

        [Test]
        public void get_header_without_a_string_token_should_just_localize_the_header()
        {
            theColumn.GetHeader().ShouldEqual(LocalizationManager.GetHeader<Case>(x => x.Condition));
        }

        [Test]
        public void if_the_column_is_not_queryable_return_no_filters()
        {
            theColumn.IsFilterable = false;
            theColumn.PossibleFilters(queryService).Any().ShouldBeFalse();
        }

        [Test]
        public void if_the_column_is_not_fetched_return_no_select_accessors()
        {
            theColumn.FetchMode = ColumnFetching.NoFetch;
            theColumn.SelectAccessors().Any().ShouldBeFalse();
        }

        [Test]
        public void return_a_single_select_accessor_if_the_mode_is_fetch()
        {
            theColumn.FetchMode = ColumnFetching.FetchOnly;
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");
            theColumn.FetchMode = ColumnFetching.FetchAndDisplay;
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");

        }
    }

    [TestFixture]
    public class when_creating_filters_for_a_filterable_column
    {
        private GridColumn<Case> theColumn;
        private QueryService queryService;
        private FilterDTO theFilter;

        [SetUp]
        public void SetUp()
        {
            theColumn = GridColumn<Case>.ColumnFor(x => x.Condition);
            theColumn.IsFilterable = true;
            
            queryService = new QueryService(new IFilterHandler[0]);

            theFilter = theColumn.PossibleFilters(queryService).Single();
        }

        [Test]
        public void the_filter_display_is_the_header_text()
        {
            theFilter.display.ShouldEqual(theColumn.GetHeader());
        }

        [Test]
        public void the_value_of_the_filter_is_the_accessor_name()
        {
            theFilter.value.ShouldEqual(theColumn.Accessor.Name);
        }

        [Test]
        public void should_have_all_the_string_operators()
        {
            // The exact list is tested by StoryTeller
            theFilter.operators.Any(x => x.value == OperatorKeys.EQUAL.Key).ShouldBeTrue();
            theFilter.operators.Any(x => x.value == OperatorKeys.NOTEQUAL.Key).ShouldBeTrue();
            theFilter.operators.Any(x => x.value == OperatorKeys.CONTAINS.Key).ShouldBeTrue();
        }
    }
}