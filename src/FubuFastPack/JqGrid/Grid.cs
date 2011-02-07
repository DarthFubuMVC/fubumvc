using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public abstract class Grid<TEntity, TService> : ISmartGrid where TEntity : DomainEntity
    {
        private readonly GridDefinition<TEntity> _definition = new GridDefinition<TEntity>();

        public GridResults Invoke(IServiceLocator services, GridDataRequest request)
        {
            var runner = services.GetInstance<IGridRunner<TEntity, TService>>();
            var source = BuildSource(runner.Service);

            return runner.RunGrid(_definition, source, request);
        }

        public IEnumerable<Criteria> BaselineCriterion
        {
            get { return new Criteria[0]; }
        }


        public IGridDefinition Definition
        {
            get { return _definition; }
        }

        protected FilterColumn<TEntity> FilterOn(Expression<Func<TEntity, object>> expression)
        {
            return _definition.AddColumn(new FilterColumn<TEntity>(expression));
        }

        protected GridColumn<TEntity> Show(Expression<Func<TEntity, object>> expression)
        {
            return _definition.Show(expression);
        }

        protected LinkColumn<TEntity> ShowViewLink(Expression<Func<TEntity, object>> expression)
        {
            return _definition.ShowViewLink(expression);
        }

        public GridDefinition<TEntity>.OtherEntityLinkExpression<TOther> ShowViewLinkForOther<TOther>(Expression<Func<TEntity, TOther>> entityProperty) where TOther : DomainEntity
        {
            return _definition.ShowViewLinkForOther(entityProperty);
        }

        public abstract IGridDataSource<TEntity> BuildSource(TService service);
    }
}