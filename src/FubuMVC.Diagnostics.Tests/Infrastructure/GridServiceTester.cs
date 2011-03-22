using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Infrastructure.Grids;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Tests.Models;
using FubuMVC.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class GridServiceTester : InteractionContext<GridService<SampleGridModel>>
    {
        private List<JsonGridRow> _rows;
        private SampleGridModel _model;
        private JsonGridQuery<SampleGridModel> _query;

        protected override void beforeEach()
        {
            _rows = new List<JsonGridRow>();
            _model = new SampleGridModel();
            _query = new JsonGridQuery<SampleGridModel>();

            MockFor<IGridRowBuilder<SampleGridModel>>()
                .Expect(builder => builder.RowsFor(_model))
                .Return(_rows.AsQueryable());
        }

        private void fillDummyRows(int nrRows)
        {
            for (int i = 0; i < nrRows; ++i)
            {
                _rows.Add(new JsonGridRow());
            }
        }

        private void fillBasicRows()
        {
            _rows.Add(new JsonGridRow
                          {
                              Columns = new List<JsonGridColumn>
                                            {
                                                new JsonGridColumn {Name = "FirstName", Value = "Josh"},
                                                new JsonGridColumn {Name = "LastName", Value = "Arnold"}
                                            }
                          });
            _rows.Add(new JsonGridRow
                          {
                              Columns = new List<JsonGridColumn>
                                            {
                                                new JsonGridColumn {Name = "FirstName", Value = "Jeremy"},
                                                new JsonGridColumn {Name = "LastName", Value = "Miller"}
                                            }
                          });
            _rows.Add(new JsonGridRow
                          {
                              Columns = new List<JsonGridColumn>
                                            {
                                                new JsonGridColumn {Name = "FirstName", Value = "Chad"},
                                                new JsonGridColumn {Name = "LastName", Value = "Myers"}
                                            }
                          });
        }

        [Test]
        public void should_return_total_number_of_records()
        {
            const int totalRecords = 100;
            fillDummyRows(totalRecords);

            ClassUnderTest
                .GridFor(_model, _query)
                .TotalRecords
                .ShouldEqual(totalRecords);
        }

        [Test]
        public void should_return_total_number_of_pages()
        {
            const int totalRecords = 100;
            fillDummyRows(totalRecords);
            _query.rows = 30;

            ClassUnderTest
                .GridFor(_model, _query)
                .TotalPages
                .ShouldEqual(4);
        }

        [Test]
        public void should_return_current_page()
        {
            const int totalRecords = 100;
            fillDummyRows(totalRecords);

            const int pageNr = 5;
            _query.page = pageNr;

            ClassUnderTest
                .GridFor(_model, _query)
                .PageNr
                .ShouldEqual(pageNr);
        }

        [Test]
        public void should_apply_single_filter()
        {
            fillBasicRows();
            _query.Filters = new List<JsonGridFilter> {new JsonGridFilter
                                                           {
                                                               ColumnName = "FirstName",
                                                               Values = new List<string> { "j" }
                                                           }};

            ClassUnderTest
                .GridFor(_model, _query)
                .Rows
                .ShouldHaveCount(2);
        }

        [Test]
        public void should_order_rows_in_ascending_order()
        {
            fillBasicRows();
            _query.sidx = "FirstName";

            ClassUnderTest
                .GridFor(_model, _query)
                .Rows
                .First()
                .FindColumn("FirstName")
                .Value
                .ShouldEqual("Chad");
        }

        [Test]
        public void should_order_rows_in_descending_order()
        {
            fillBasicRows();
            _query.sidx = "LastName";
            _query.sord = JsonGridQuery.DESCENDING;

            ClassUnderTest
                .GridFor(_model, _query)
                .Rows
                .First()
                .FindColumn("LastName")
                .Value
                .ShouldEqual("Myers");
        }

        [Test]
        public void should_apply_paging()
        {
            fillBasicRows();
            _query.page = 2;
            _query.rows = 1;

            var grid = ClassUnderTest.GridFor(_model, _query);

            grid.Rows.ShouldHaveCount(1);
            grid.TotalRecords.ShouldEqual(3);
            grid.TotalPages.ShouldEqual(3);
        }
    }
}