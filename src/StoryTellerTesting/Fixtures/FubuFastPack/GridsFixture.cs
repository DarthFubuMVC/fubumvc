using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using FubuFastPack.Querying;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestApplication;
using FubuTestApplication.Domain;
using IntegrationTesting.FubuFastPack;
using NHibernate;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Engine.Sets;
using StructureMap;

namespace IntegrationTesting.Fixtures.FubuFastPack
{
    public class GridsFixture : Fixture
    {
        private IContainer _container;
        private GridResults _lastResults;
        private GridDataRequest _paging = new GridDataRequest(1, 100, null, true);
        private IRepository _repository;
        private ITransactionBoundary _boundary;

        public GridsFixture()
        {
            checkColumns(1);
            checkColumns(2);
            checkColumns(3);
            checkColumns(4);

            runGrid<OneColumnGrid>();
            runGrid<OneColumnRepositoryGrid>();

            SelectionValuesFor("operations").AddRange(OperatorKeys.Keys.Select(x => x.Key));
            SelectionValuesFor("fields").AddRange(
                typeof (Case).GetProperties().Where(x => x.DeclaringType == typeof (Case)).Select(x => x.Name));
        }


        public override void SetUp(ITestContext context)
        {
            DatabaseDriver.Bootstrap(true);
            _container = DatabaseDriver.ContainerWithDatabase();
            _container.Configure(x =>
            {
                x.AddRegistry(new FastPackRegistry());
                x.For<IObjectConverter>().Use<ObjectConverter>();
            });

            FubuApplication.For<FubuRegistry>().StructureMap(() => _container).Bootstrap();

            _boundary = _container.GetInstance<ITransactionBoundary>();
            _boundary.Start();
            _repository = _container.GetInstance<IRepository>();
        }

        public override void TearDown()
        {
            _container.Dispose();
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
                x.SetProperty(c => c.Number, "100");

                x.Do = c => _repository.Save(c);
            }).AsTable("If the cases are").After(() => _container.GetInstance<ISession>().Flush());
        }

        [FormatAs("Fetch page {pageNumber} with {resultsPerPage} results per page")]
        public void LookForPage(int pageNumber, int resultsPerPage)
        {
            _paging = new GridDataRequest(pageNumber, resultsPerPage, null, true);
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

        private void runGrid<T>() where T : ISmartGrid
        {
            this[typeof (T).Name] = Do("With grid " + typeof (T).Name, () =>
            {
                var grid = _container.GetInstance<T>();
                Debug.WriteLine("Fetching Grid {0} for {1}", typeof (T).Name, _paging);

                _lastResults = grid.Invoke(new StructureMapServiceLocator(_container), _paging);
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

        [ExposeAsTable("Apply Criteria to a Projection Grid")]
        [FormatAs("{Property}{Operator}{Value}{Identifier}{IsReturned}")]
        public bool IsReturned([SelectionValues("fields")] string Property,
                               [SelectionValues("operations")] string Operator, string Value, string Identifier)
        {
            var paging = new GridDataRequest(1, 20, null, true);
            paging.Criterion = new[]{
                new Criteria{
                    op = Operator,
                    property = Property,
                    value = Value
                }
            };

            var grid = _container.GetInstance<FilterableRepositoryGrid>();
            var results = grid.Invoke(new StructureMapServiceLocator(_container), paging);

            Debug.WriteLine("Results are:");
            results.items.Each(item => Debug.WriteLine(item.cell.Select(x => x.ToString()).Join(" | ")));
            Debug.WriteLine("");

            return results.items.Any(x => x.cell.Contains(Identifier));
        }

        private void checkColumns(int count)
        {
            var setVerificationGrammar = VerifyDataTable(() => tableForColumns(count))
                .Titled("Check {0} columns".ToFormat(count))
                .Columns(x => setupComparer(count, x));

            setVerificationGrammar.Ordered = true;

            this["Check{0}Columns".ToFormat(count)] =
                setVerificationGrammar;
        }

        private void setupComparer(int count, IDataRowComparer comparer)
        {
            for (var i = 1; i <= count; i++)
            {
                var columnName = "Column" + i;
                comparer.MatchOn<string>(columnName);
            }
        }

        private DataTable tableForColumns(int count)
        {
            var table = new DataTable();
            for (var i = 1; i <= count; i++)
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


        #region Nested type: FilterableRepositoryGrid

        public class FilterableRepositoryGrid : RepositoryGrid<Case>
        {
            public FilterableRepositoryGrid()
            {
                Show(x => x.Identifier);

                FilterOn(x => x.CaseType);
                FilterOn(x => x.Condition);
                FilterOn(x => x.Priority);
                FilterOn(x => x.Status);
                FilterOn(x => x.Title);
                FilterOn(x => x.Number);
            }
        }

        #endregion

        #region Nested type: OneColumnGrid

        public class OneColumnGrid : ProjectionGrid<Case>
        {
            public OneColumnGrid()
            {
                Show(x => x.Identifier);
            }
        }

        #endregion

        #region Nested type: OneColumnRepositoryGrid

        public class OneColumnRepositoryGrid : RepositoryGrid<Case>
        {
            public OneColumnRepositoryGrid()
            {
                Show(x => x.Identifier);
            }
        }

        #endregion

        public class FilterOperatorRow
        {
            public string Property { get; set; }
            public string Operator {get; set;}
        }
    }
}