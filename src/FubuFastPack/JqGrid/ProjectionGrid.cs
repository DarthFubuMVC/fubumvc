using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
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

    public class ProjectionDataSource<T> : IGridDataSource where T : DomainEntity
    {
        private readonly Projection<T> _projection;

        public ProjectionDataSource(Projection<T> projection)
        {
            _projection = projection;
        }

        public int TotalCount()
        {
            return _projection.Count();
        }

        public IGridData Fetch(PagingOptions options)
        {
            var records = _projection.ExecuteCriteriaWithProjection(options).Cast<object>().ToList();
            var accessors = _projection.SelectAccessors().ToList();

            return new ProjectionGridData(records, accessors);
        }
    }

    public class ProjectionGridData : IGridData
    {
        private readonly IList<Accessor> _accessors;
        private readonly Queue<object> _records;
        private object[] _currentRow;

        public ProjectionGridData(IList<object> records, IList<Accessor> accessors)
        {
            records.Each(x => Debug.WriteLine(x.GetType().Name));

            _records = new Queue<object>(records);
            _accessors = accessors;
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            var index = _accessors.IndexOf(accessor);
            return () => _currentRow[index];
        }

        public bool MoveNext()
        {
            if (_records.Any())
            {
                _currentRow = (object[])_records.Dequeue();
                return true;
            }

            return false;
        }
    }

}