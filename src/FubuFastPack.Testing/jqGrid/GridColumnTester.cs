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
            theColumn.FilteredProperties().Any().ShouldBeFalse();
        }


        [Test]
        public void return_a_single_select_accessor()
        {
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");

        }
    }

}