using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class ProjectionGrid<T> : IGrid where T : DomainEntity
    {
        private readonly IList<Action<Projection<T>>> _projectionBuilders = new List<Action<Projection<T>>>();
        private readonly IList<IGridColumn> _columns = new List<IGridColumn>();

        public ProjectionGrid()
        {
            alterProjection = p => p.AddColumn(x => x.Id);
        }

        protected ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            return new ColumnExpression(this, expression);
        }

        private Action<Projection<T>> alterProjection
        {
            set
            {
                _projectionBuilders.Add(value);
            }
        }

        public IEnumerable<IGridColumn> Columns
        {
            get { return _columns; }
        }

        public IGridDataSource BuildSource(IServiceLocator services)
        {
            var projection = services.GetInstance<Projection<T>>();
            // TODO -- what to do about data restrictions?

            _projectionBuilders.Each(x => x(projection));

            return new ProjectionDataSource<T>(projection);
        }

        public class ColumnExpression
        {
            private readonly GridColumn _column;

            public ColumnExpression(ProjectionGrid<T> grid, Expression<Func<T, object>> expression)
            {
                var accessor = expression.ToAccessor();
                _column = new GridColumn(accessor);
                grid._columns.Add(_column);
                grid.alterProjection = p => p.AddColumn(accessor);
            }
        }
    }
}