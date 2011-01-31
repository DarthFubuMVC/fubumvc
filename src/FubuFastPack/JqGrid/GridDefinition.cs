using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Querying;
using FubuLocalization;

namespace FubuFastPack.JqGrid
{
    public class GridDefinition<T> : IGridDefinition
    {
        private readonly IList<IGridColumn> _columns = new List<IGridColumn>();
        private readonly IList<Accessor> _filterableAccessors = new List<Accessor>();

        private readonly Cache<string, Expression<Func<T, object>>> _properties =
            new Cache<string, Expression<Func<T, object>>>();

        private readonly IList<Accessor> _selectedAccessors = new List<Accessor>();

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IEnumerable<Accessor> SelectedAccessors
        {
            get { return _selectedAccessors; }
        }

        // TODO -- think we'll need an actual Filterable column here for labels
        // maybe IGridColumn just gets a Filterable / Sortable / Displayed props
        public IEnumerable<FilterDTO> AllPossibleFilters(IQueryService queryService)
        {
            return _filterableAccessors.Select(x =>
            {
                return new FilterDTO(){
                    display = LocalizationManager.GetHeader((PropertyInfo) x.InnerProperty),
                    value = x.Name,
                    operators = queryService.FilterOptionsFor(_properties[x.Name]).Select(o => o.ToOperator()).ToArray()
                };
            });
        }

        protected void addColumn(IGridColumn column)
        {
            _columns.Add(column);
        }

        // TODO -- might make sense to demote this to private
        public void AddSelectedAccessor(Accessor accessor)
        {
            _selectedAccessors.Add(accessor);
        }

        // TODO -- might make sense to demote this to private
        public void AddFilterableAccessor(Accessor accessor)
        {
            _filterableAccessors.Add(accessor);
        }


        public ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            return new ColumnExpression(this, expression);
        }

        public Expression<Func<T, object>> PropertyFor(string propertyName)
        {
            // TODO -- better error handling here?
            return _properties[propertyName];
        }

        public void FilterOn(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();

            _properties[accessor.Name] = expression;
            AddFilterableAccessor(accessor);
        }

        #region Nested type: ColumnExpression

        public class ColumnExpression
        {
            private readonly GridColumn _column;

            public ColumnExpression(GridDefinition<T> grid, Expression<Func<T, object>> expression)
            {
                var accessor = expression.ToAccessor();

                grid._properties[accessor.Name] = expression;
                _column = new GridColumn(accessor);
                grid.addColumn(_column);
                grid.AddSelectedAccessor(accessor);
            }
        }

        #endregion
    }
}