using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly GridDefinition<T> _definition = new GridDefinition<T>();

        public ProjectionGrid()
        {
            var accessor = ReflectionHelper.GetAccessor<T>(x => x.Id);
            _definition.AddSelectedAccessor(accessor);
        }

        protected GridDefinition<T>.ColumnExpression Show(Expression<Func<T, object>> expression)
        {
            return _definition.Show(expression);
        }

        private Action<Projection<T>> alterProjection
        {
            set
            {
                _projectionBuilders.Add(value);
            }
        }

        public GridDefinition Definition
        {
            get { return _definition; }
        }

        public IGridDataSource BuildSource(IServiceLocator services)
        {
            var projection = services.GetInstance<Projection<T>>();
            // TODO -- what to do about data restrictions?
            
            _definition.SelectedAccessors.Each(a => projection.AddColumn(a));
            _projectionBuilders.Each(x => x(projection));

            return new ProjectionDataSource<T>(projection);
        }


    }
}