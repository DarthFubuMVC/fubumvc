using System;
using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using NHibernate;
using NUnit.Framework;
using System.Linq;
using FubuMVC.Tests;
using System.Collections.Generic;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class GridTester
    {
        [Test]
        public void do_not_allow_user_sorting_overwrites_sortable_on_all_columns()
        {
            var grid = new FakeCaseGrid();
            grid.DoNotAllowUserSorting();

            var model = grid.ToColumnModel();
            model.Any().ShouldBeTrue();

            model.Each(dict => dict["sortable"].ShouldEqual(false));
        }
    }

    public class FakeCaseGrid : Grid<Case, ISession>
    {
        public FakeCaseGrid()
        {
            Show(x => x.Condition).Sortable(true);
            Show(x => x.IsSecret).Sortable(true);
            Show(x => x.Reason).Sortable(true);
        }

        public override IGridDataSource<Case> BuildSource(ISession service)
        {
            throw new NotImplementedException();
        }
    }
}