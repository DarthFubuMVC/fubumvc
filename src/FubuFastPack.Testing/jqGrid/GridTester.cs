using System;
using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NHibernate;
using NUnit.Framework;
using System.Linq;
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
        public void if_the_header_is_null_get_header_returns_pluralization_of_the_entity_type()
        {
            var grid = new WithoutHeaderGrid();
            grid.GetHeader().ShouldEqual("en-US_Case_PLURAL");
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
        public void can_save_query_is_true_by_default()
        {
            var grid = new FakeCaseGrid();
            grid.Definition.CanSaveQuery.ShouldBeTrue();           
        }

        [Test]
        public void can_save_query()
        {
            var grid = new FakeCaseGrid();

            grid.CanSaveQuery(false);
            grid.Definition.CanSaveQuery.ShouldBeFalse();

        }

    }

    public class WithoutHeaderGrid : Grid<Case, ISession>
    {
        public override IGridDataSource<Case> BuildSource(ISession service)
        {
            throw new NotImplementedException();
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