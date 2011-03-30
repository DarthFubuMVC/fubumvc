using FubuFastPack.JqGrid;
using FubuTestApplication.Grids;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class GridRequestTester
    {
        [Test]
        public void default_page_is_one()
        {
            new GridRequest<CaseGrid>().page.ShouldEqual(1);
        }

        [Test]
        public void default_rows_is_ten()
        {
            new GridRequest<CaseGrid>().rows.ShouldEqual(10);
        }

        [Test]
        public void default_sort_field_is_null()
        {
            new GridRequest<CaseGrid>().sidx.ShouldBeNull();
        }

        [Test]
        public void to_data_request_with_ascending_sort_order()
        {
            var request = new GridRequest<CaseGrid>(){
                page = 2,
                rows = 15,
                sidx = "field1",
                sord = "asc"
            }.ToDataRequest();

            request.Page.ShouldEqual(2);
            request.ResultsPerPage.ShouldEqual(15);
            request.SortColumn.ShouldEqual("field1");
            request.SortAscending.ShouldBeTrue();
        }


        [Test]
        public void to_data_request_with_descending_sort_order()
        {
            var request = new GridRequest<CaseGrid>()
            {
                page = 2,
                rows = 15,
                sidx = "field1",
                sord = "desc"
            }.ToDataRequest();

            request.Page.ShouldEqual(2);
            request.ResultsPerPage.ShouldEqual(15);
            request.SortColumn.ShouldEqual("field1");
            request.SortAscending.ShouldBeFalse();
        }
    }
}