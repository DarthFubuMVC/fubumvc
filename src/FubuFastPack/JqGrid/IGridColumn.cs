using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class GridDefinition
    {
        private readonly IList<IGridColumn> _columns = new List<IGridColumn>();
        private readonly IList<Accessor> _selectedAccessors = new List<Accessor>();

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

        public IEnumerable<Accessor> SelectedAccessors
        {
            get
            {
                Debug.WriteLine("There are {0} selected accessors", _selectedAccessors.Count );
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
    }


    public interface IGridColumn
    {
        GridColumnDTO ToDto();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);
    }


}