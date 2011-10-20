using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Core.Grids.Columns;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Tests.Models;
using FubuMVC.Tests;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class GridServiceTester : InteractionContext<GridService<SampleGridModel, SampleGridRowModel>>
    {
        private List<JsonGridRow> _rows;
        private SampleGridModel _model;
        private JsonGridQuery _query;

        protected override void beforeEach()
        {
            _rows = new List<JsonGridRow>();
            _model = new SampleGridModel();
            _query = new JsonGridQuery();

            MockFor<IGridRowBuilder<SampleGridModel, SampleGridRowModel>>()
                .Expect(builder => builder.RowsFor(_model, _query.Filters))
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

    [TestFixture]
    public class GridExtensionsTester
    {
        [Test]
        public void should_apply_filters()
        {
            var graph = ObjectMother.DiagnosticsGraph();
            var queryFilters = new List<JsonGridFilter> { new JsonGridFilter { ColumnName = "InputModel", Values = new List<string> { typeof(DashboardRequestModel).Name } } };

        	var columns = new List<IGridColumn<BehaviorChain>> {new InputModelColumn()};
            var gridFilters = new List<IGridFilter<BehaviorChain>> {new TestInputModelFilter()};

            graph
                .Behaviors
                .Single(chain => gridFilters.Matches(queryFilters, chain))
                .InputType()
                .ShouldEqual(typeof (DashboardRequestModel));
        }

        public class TestInputModelFilter : IGridFilter<BehaviorChain>
        {
            public bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
            {
                return filter.ColumnName.Equals("InputModel", StringComparison.OrdinalIgnoreCase);
            }

            public bool Matches(BehaviorChain target, JsonGridFilter filter)
            {
                return target.InputType() != null && filter.Values.Any(v => target.InputType().Name.ToLower().Contains(v.ToLower()));
            }
        }
    }
}