using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Persistence;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;
using FubuCore;

namespace FubuFastPack.JqGrid
{
    public abstract class RepositoryGrid<T> : IGrid
    {
        private readonly IList<IGridColumn> _columns = new List<IGridColumn>();
        private readonly Cache<string, Expression<Func<T, object>>> _sortables
            = new Cache<string, Expression<Func<T, object>>>();
        

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IGridDataSource BuildSource(IServiceLocator services)
        {
            var repository = services.GetInstance<IRepository>();
            return new RepositoryDataSource<T>(this, repository);
        }

        protected virtual IQueryable<T> query(IRepository repository)
        {
            return repository.Query<T>();
        }

        protected ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            return new ColumnExpression(this, expression);
        }

        public class ColumnExpression
        {
            private readonly GridColumn _column;

            public ColumnExpression(RepositoryGrid<T> grid, Expression<Func<T, object>> expression)
            {
                var accessor = expression.ToAccessor();
                _column = new GridColumn(accessor);
                grid._columns.Add(_column);
                grid._sortables[expression.GetName()] = expression;
            }
        }

        public class RepositoryDataSource<T> : IGridDataSource
        {
            private readonly RepositoryGrid<T> _grid;
            private readonly IRepository _repository;

            public RepositoryDataSource(RepositoryGrid<T> grid, IRepository repository)
            {
                _grid = grid;
                _repository = repository;
            }

            public int TotalCount()
            {
                return query().Count();
            }

            private IQueryable<T> query()
            {
                return _grid.query(_repository);
            }

            public IGridData Fetch(PagingOptions options)
            {
                var queryable = query();
                queryable = sort(queryable, options);
                queryable = applyPaging(queryable, options);

                // apply sorting here
                // apply paging here


                return new EntityGridData<T>(queryable);
            }

            private IQueryable<T> applyPaging(IQueryable<T> queryable, PagingOptions options)
            {
                return queryable.Skip(options.ResultsToSkip()).Take(options.ResultsPerPage);
            }

            // TODO -- default sorting?
            private IQueryable<T> sort(IQueryable<T> queryable, PagingOptions options)
            {
                if (options.SortColumn.IsNotEmpty())
                {
                    var expression = _grid._sortables[options.SortColumn];
                    return options.SortAscending 
                        ? queryable.OrderBy(expression) 
                        : queryable.OrderByDescending(expression);
                }

                return queryable;
            }
        }
    }
}