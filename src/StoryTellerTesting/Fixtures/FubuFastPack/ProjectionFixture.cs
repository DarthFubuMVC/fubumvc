using System;
using System.Data;
using System.Diagnostics;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using FubuFastPack.Querying;
using FubuMVC.Core;
using IntegrationTesting.Domain;
using IntegrationTesting.FubuFastPack;
using NHibernate;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Engine.Sets;
using StructureMap;
using FubuCore;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.StructureMap;

namespace IntegrationTesting.Fixtures.FubuFastPack
{
    public class ProjectionFixture : Fixture
    {
        private IContainer _container;
        private IRepository _repository;
        private PagingOptions _paging = new PagingOptions(1, 100, null, true);
        private GridResults _lastResults;

        public ProjectionFixture()
        {
            checkColumns(1);
            checkColumns(2);
            checkColumns(3);
            checkColumns(4);

            runGrid<OneColumnGrid>();
            runGrid<OneColumnRepositoryGrid>();
        }


        public override void SetUp(ITestContext context)
        {
            DatabaseDriver.Bootstrap();
            _container = DatabaseDriver.ContainerWithDatabase();
            FubuApplication.For<FubuRegistry>().StructureMap(() => _container).Bootstrap();

            _repository = _container.GetInstance<IRepository>();


        }

        public IGrammar CasesAre()
        {
            return CreateNewObject<Case>(x =>
            {
                x.CreateNew<Case>();
                x.SetProperty(c => c.Identifier, "001");
                x.SetProperty(c => c.CaseType, "Question");
                x.SetProperty(c => c.Condition, "Open");
                x.SetProperty(c => c.Priority, "Urgent");
                x.SetProperty(c => c.Status, "Working");
                x.SetProperty(c => c.Title, "Important Case");

                x.Do = c => _repository.Save(c);
            }).AsTable("If the cases are").After(() => _container.GetInstance<ISession>().Flush());
        }

        [FormatAs("Fetch page {pageNumber} with {resultsPerPage} results per page")]
        public void LookForPage(int pageNumber, int resultsPerPage)
        {
            _paging = new PagingOptions(pageNumber, resultsPerPage, null, true);
        }

        [FormatAs("Sort ascending by {field}")]
        public void SortAscendingBy(string field)
        {
            _paging.SortAscending = true;
            _paging.SortColumn = field;
        }

        [FormatAs("Sort descending by {field}")]
        public void SortDescendingBy(string field)
        {
            _paging.SortAscending = false;
            _paging.SortColumn = field;
        }

        private void runGrid<T>() where T : IGrid
        {
            this[typeof(T).Name] = Do("With grid " + typeof(T).Name, () =>
            {
                var grid = _container.GetInstance<T>();
                var runner = _container.GetInstance<GridRunner>();
                Debug.WriteLine("Fetching Grid {0} for {1}", typeof (T).Name, _paging);
                _lastResults = runner.Fetch(_paging, grid);
                Debug.WriteLine(_lastResults);
            });
        }

        public IGrammar CheckPaging()
        {
            return VerifyPropertiesOf<GridResults>("The paging results should be", x =>
            {
                x.Object = () => _lastResults;
                x.Check(o => o.page);
                x.Check(o => o.total);
                x.Check(o => o.records);
            });
        }

        private void checkColumns(int count)
        {
            SetVerificationGrammar setVerificationGrammar = VerifyDataTable(() => tableForColumns(count))
                .Titled("Check {0} columns".ToFormat(count))
                .Columns(x => setupComparer(count, x));

            setVerificationGrammar.Ordered = true;

            this["Check{0}Columns".ToFormat(count)] =
                setVerificationGrammar;
        }

        private void setupComparer(int count, IDataRowComparer comparer)
        {
            for (int i = 1; i <= count; i++)
            {
                var columnName = "Column" + i;
                comparer.MatchOn<string>(columnName);
            }
        }

        private DataTable tableForColumns(int count)
        {
            var table = new DataTable();
            for (int i = 1; i <= count; i++)
            {
                table.Columns.Add("Column" + i, typeof (string));
            }

            _lastResults.items.Each(r =>
            {
                var values = r.cell.Skip(1).Take(count).ToArray();
                table.Rows.Add(values);
            });

            return table;
        }

        public class OneColumnGrid : ProjectionGrid<Case>
        {
            public OneColumnGrid()
            {
                Show(x => x.Identifier);
            }
        }

        public class OneColumnRepositoryGrid : RepositoryGrid<Case>
        {
            public OneColumnRepositoryGrid()
            {
                Show(x => x.Identifier);
            }
        }
    }
}