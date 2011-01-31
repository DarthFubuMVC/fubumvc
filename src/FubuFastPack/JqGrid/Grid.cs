using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public abstract class Grid<TEntity, TService> : IGrid where TEntity : DomainEntity
    {
        private readonly GridDefinition<TEntity> _definition = new GridDefinition<TEntity>();

        private readonly Cache<string, Expression<Func<TEntity, object>>> _sortables
            = new Cache<string, Expression<Func<TEntity, object>>>();

        protected Grid()
        {
            var accessor = ReflectionHelper.GetAccessor<TEntity>(x => x.Id);
            _definition.AddSelectedAccessor(accessor);
        }

        public GridResults Invoke(IServiceLocator services, PagingOptions request)
        {
            var runner = services.GetInstance<IGridRunner<TEntity, TService>>();
            var source = BuildSource(runner.Service);

            return runner.RunGrid(_definition, source, request);
        }

        

        public GridDefinition Definition
        {
            get { return _definition; }
        }

        protected GridDefinition<TEntity>.ColumnExpression Show(Expression<Func<TEntity, object>> expression)
        {
            _sortables[expression.GetName()] = expression;
            return _definition.Show(expression);
        }

        public abstract IGridDataSource<TEntity> BuildSource(TService service);

        protected Cache<string, Expression<Func<TEntity, object>>> sortables
        {
            get { return _sortables; }
        }
    }
}