using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using System.Linq;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class GridDefinitionTester
    {
        public class TargetGrid : GridDefinition<GridDefTarget>{}

        [Test]
        public void create_column_json()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool);
            grid.Column(x => x.Name);

            var json = grid.As<IGridDefinition>().ToColumnJson();

            json
                .ShouldEqual("[{name: \"Count\", field: \"Count\", id: \"Count\", sortable: true}, {name: \"IsCool\", field: \"IsCool\", id: \"IsCool\", sortable: true}, {name: \"Name\", field: \"Name\", id: \"Name\", sortable: true}]");
        }

        [Test]
        public void format_data_smoke_test()
        {
            var grid = new TargetGrid();
            grid.Column(x => x.Count);
            grid.Column(x => x.IsCool);
            grid.Column(x => x.Name);

            var data = new GridDefTarget[]{
                new GridDefTarget{Count = 1, IsCool = true, Name = "Scooby"},
                new GridDefTarget{Count = 2, IsCool = true, Name = "Velma"},
                new GridDefTarget{Count = 3, IsCool = true, Name = "Daphne"},
            };

            var dicts = grid.As<IGridDefinition<GridDefTarget>>().FormatData(data);
            dicts.Select(x => x["Name"]).ShouldHaveTheSameElementsAs("Scooby", "Velma", "Daphne");
            dicts.Select(x => x["Count"]).ShouldHaveTheSameElementsAs(1, 2, 3);
        }

        [Test]
        public void throws_if_source_cannot_work_1()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<NotASource>();
            });
        }

        [Test]
        public void throws_if_source_cannot_work_2()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<WrongSource>();
            });
        }

        [Test]
        public void throws_if_source_cannot_work_3()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new TargetGrid().SourceIs<MoreWrongSource>();
            });
        }

        [Test]
        public void source_happy_path_without_query_object()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<SimpleGoodSource>();

            targetGrid.SourceType.ShouldEqual(typeof (SimpleGoodSource));
        }

        [Test]
        public void source_happy_path_with_query_object()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<QueryGoodSource>();

            targetGrid.SourceType.ShouldEqual(typeof (QueryGoodSource));
        }

        [Test]
        public void select_data_source_url_with_no_data_source()
        {
            new TargetGrid().As<IGridDefinition>().SelectDataSourceUrl(new StubUrlRegistry())
                .ShouldBeNull();
        }

        [Test]
        public void select_data_source_url_with_source_type_and_no_query()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<SimpleGoodSource>();

            targetGrid.DetermineRunnerType().ShouldEqual(typeof(GridRunner<GridDefTarget, TargetGrid,SimpleGoodSource>));
        }

        [Test]
        public void select_data_source_url_with_source_type_and_a_query()
        {
            var targetGrid = new TargetGrid();
            targetGrid.SourceIs<QueryGoodSource>();

            targetGrid.DetermineRunnerType().ShouldEqual(typeof(GridRunner<GridDefTarget, TargetGrid, QueryGoodSource, DifferentClass>));
        }


        public class NotASource{}
        public class WrongSource : IGridDataSource<DifferentClass>
        {
            public IEnumerable<DifferentClass> GetData()
            {
                throw new NotImplementedException();
            }
        }

        public class MoreWrongSource : IGridDataSource<DifferentClass, DifferentClass>
        {
            public IEnumerable<DifferentClass> GetData(DifferentClass query)
            {
                throw new NotImplementedException();
            }
        }

        public class SimpleGoodSource : IGridDataSource<GridDefTarget>
        {
            public IEnumerable<GridDefTarget> GetData()
            {
                throw new NotImplementedException();
            }
        }

        public class QueryGoodSource : IGridDataSource<GridDefTarget, DifferentClass>
        {
            public IEnumerable<GridDefTarget> GetData(DifferentClass query)
            {
                throw new NotImplementedException();
            }
        }


        public class DifferentClass{}

        public class GridDefTarget
        {
            public string Name { get; set; }
            public bool IsCool { get; set; }
            public int Count { get; set; }
        }
    }
}