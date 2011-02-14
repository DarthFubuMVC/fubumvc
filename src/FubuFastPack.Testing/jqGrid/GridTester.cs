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

        [Test]
        public void get_header_by_default_uses_the_string_token()
        {
            new FakeCaseGrid().GetHeader().ShouldEqual(CommandKey.Action.ToString());
        }

        [Test]
        public void allow_create_new()
        {
            var grid = new FakeCaseGrid();
            grid.Definition.AllowCreationOfNew.ShouldBeFalse();

            grid.AllowCreateNew();
            grid.Definition.AllowCreationOfNew.ShouldBeTrue();
        }

        [Test]
        public void can_save_query()
        {
            var grid = new FakeCaseGrid();
            grid.Definition.CanSaveQuery.ShouldBeFalse();

            grid.CanSaveQuery();
            grid.Definition.CanSaveQuery.ShouldBeTrue();

        }
    }

    public class FakeCaseGrid : Grid<Case, ISession>
    {
        public FakeCaseGrid()
        {
            Header = CommandKey.Action;

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