using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class GridDefinition<T> : IGridDefinition
    {
        private readonly List<IGridColumn> _columns = new List<IGridColumn>();

        private readonly Cache<string, Expression<Func<T, object>>> _properties =
            new Cache<string, Expression<Func<T, object>>>();

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IEnumerable<Accessor> SelectedAccessors
        {
            get { return _columns.SelectMany(x => x.SelectAccessors()); }
        }

        public IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService)
        {
            return _columns.SelectMany(x => x.PossibleFilters(queryService));
        }

        protected void addColumn(IGridColumn column)
        {
            _columns.Add(column);
        }

        // TODO -- get rid of duplication here.
        public ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            _properties[accessor.Name] = expression;

            var column = new GridColumn<T>(accessor, expression){
                FetchMode = ColumnFetching.FetchAndDisplay
            };

            _columns.Add(column);

            return new ColumnExpression(this, column);
        }

        public void FilterOn(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            _properties[accessor.Name] = expression;

            var column = new GridColumn<T>(accessor, expression){
                FetchMode = ColumnFetching.NoFetch,
                IsFilterable = true
            };

            _columns.Add(column);
        }


        public void Fetch(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();
            _properties[accessor.Name] = expression;

            var column = new GridColumn<T>(accessor, expression){
                FetchMode = ColumnFetching.FetchOnly,
                IsFilterable = false
            };

            _columns.Add(column);
        }

        #region Nested type: ColumnExpression

        public class ColumnExpression
        {
            private readonly GridColumn<T> _column;

            public ColumnExpression(GridDefinition<T> grid, GridColumn<T> column)
            {
                _column = column;
            }
        }

        #endregion

        public Expression<Func<T, object>> PropertyExpressionFor(string propertyName)
        {
            return _properties[propertyName];
        }
    }
}