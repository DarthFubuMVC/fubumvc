using System.Collections.Generic;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Tests;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class RowComparerTester
    {
        [Test]
        public void should_return_zero_when_comparing_rows_if_column_does_not_exist()
        {
            var xRow = new JsonGridRow();
            var yRow = new JsonGridRow();

            RowComparer
                .CompareRows("Test", xRow, yRow)
                .ShouldEqual(0);
        }

        [Test]
        public void should_compare_column_values_when_columns_exist()
        {
            var colName = "Test";
            var xRow = new JsonGridRow
                           {
                               Columns = new List<JsonGridColumn> {new JsonGridColumn {Name = colName, Value = "Column 2"}}
                           };
            var yRow = new JsonGridRow
                           {
                               Columns =
                                   new List<JsonGridColumn> {new JsonGridColumn {Name = colName, Value = "Column 1"}}
                           };

            RowComparer
                .CompareRows(colName, xRow, yRow)
                .ShouldEqual(1);
        }
    }
}