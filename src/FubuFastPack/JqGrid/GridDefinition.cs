using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Querying;
using FubuLocalization;

namespace FubuFastPack.JqGrid
{
    public class GridDefinition<T> : IGridDefinition
    {
        private readonly Cache<string, IGridColumn> _columns = new Cache<string, IGridColumn>();

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IEnumerable<Accessor> SelectedAccessors
        {
            get { return _columns.Where(x => x.FetchMode != ColumnFetching.NoFetch).Select(x => x.Accessor); }
        }

        // TODO -- think we'll need an actual Filterable column here for labels
        // maybe IGridColumn just gets a Filterable / Sortable / Displayed props
        public IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService)
        {
            return _columns.Where(x => x.IsFilterable).Select(x =>
            {
                return new FilterDTO{
                    display = LocalizationManager.GetHeader(x.Accessor.InnerProperty),
                    value = x.Accessor.Name,
                    operators =
                        queryService.FilterOptionsFor(x.PropertyExpressionFor<T>()).Select(o => o.ToOperator()).ToArray()
                };
            });
        }

        protected void addColumn(IGridColumn column)
        {
            _columns[column.Accessor.Name] = column;
        }


        public ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            var column = withColumn(expression, col => col.FetchMode = ColumnFetching.FetchAndDisplay);
            return new ColumnExpression(this, column);
        }

        public IGridColumn ColumnFor(string propertyName)
        {
            return _columns.First(x => x.Accessor.Name == propertyName);
        }

        public void FilterOn(Expression<Func<T, object>> expression)
        {
            withColumn(expression, col => col.IsFilterable = true);
        }

        private IGridColumn withColumn(Expression<Func<T, object>> expression, Action<IGridColumn> configure)
        {
            var accessor = expression.ToAccessor();

            if (!_columns.Has(accessor.Name))
            {
                _columns[accessor.Name] = new GridColumn(accessor, expression);
            }

            var column = _columns[accessor.Name];
            configure(column);

            return column;
        }


        public class ColumnExpression
        {
            private readonly IGridColumn _column;

            public ColumnExpression(GridDefinition<T> grid, IGridColumn column)
            {
                _column = column;
            }
        }

        public ColumnExpression Fetch(Expression<Func<T, object>> expression)
        {
            var column = withColumn(expression, col => col.FetchMode = ColumnFetching.FetchOnly);
            return new ColumnExpression(this, column);
        }
    }
}