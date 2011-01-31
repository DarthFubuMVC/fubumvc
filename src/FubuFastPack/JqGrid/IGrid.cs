using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGrid
    {
        GridDefinition Definition { get; }
        GridResults Invoke(IServiceLocator services, PagingOptions request);
    }


    public class GridDefinition
    {
        private readonly IList<IGridColumn> _columns = new List<IGridColumn>();
        private readonly IList<Accessor> _selectedAccessors = new List<Accessor>();
        private readonly IList<Accessor> _filterableAccessors = new List<Accessor>();

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        protected void addColumn(IGridColumn column)
        {
            _columns.Add(column);
        }

        public void AddSelectedAccessor(Accessor accessor)
        {
            _selectedAccessors.Add(accessor);
        }

        public void AddFilterableAccessor(Accessor accessor)
        {
            _filterableAccessors.Add(accessor);
        }

        public IEnumerable<Accessor> SelectedAccessors
        {
            get
            {
                return _selectedAccessors;
            }
        }
    }

    public class GridDefinition<T> : GridDefinition
    {
        private readonly Cache<string, Expression<Func<T, object>>> _properties = new Cache<string, Expression<Func<T, object>>>();

        public ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            return new ColumnExpression(this, expression);
        }

        public Expression<Func<T, object>> PropertyFor(string propertyName)
        {
            // TODO -- better error handling here?
            return _properties[propertyName];
        }

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
                //grid.alterProjection = p => p.AddColumn(accessor);
            }
        }

        public void FilterOn(Expression<Func<T, object>> expression)
        {
            var accessor = expression.ToAccessor();

            _properties[accessor.Name] = expression;
            AddFilterableAccessor(accessor);
        }
    }
}