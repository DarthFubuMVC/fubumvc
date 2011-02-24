using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using NHibernate;
using NHibernate.Criterion;

namespace FubuFastPack.JqGrid
{
    public enum ProjectionSelectMode
    {
        OnlySpecifiedColumns,
        WholeEntities
    }

    public abstract class ProjectionGrid<TEntity> : Grid<TEntity, ISession> where TEntity : DomainEntity
    {
        private readonly IList<Action<Projection<TEntity>>> _projectionBuilders = new List<Action<Projection<TEntity>>>();

        public ProjectionGrid()
        {
            SelectMode = ProjectionSelectMode.OnlySpecifiedColumns;
        }

        public ProjectionSelectMode SelectMode { get; set; }

        public void AddDiscriminatorColumn()
        {
            alterProjection = p => p.AddColumn(new DiscriminatorProjectionColumn<TEntity>());
        }

        protected Action<Projection<TEntity>> alterProjection
        {
            set { _projectionBuilders.Add(value); }
        }

        public override IGridDataSource<TEntity> BuildSource(ISession session)
        {
            var projection = new Projection<TEntity>(session);


            if (SelectMode == ProjectionSelectMode.OnlySpecifiedColumns)
            {
                Definition.SelectedAccessors.Each(a => projection.AddColumn(a));
                Definition.Columns.Where(c => c.IsOuterJoin).SelectMany(c => c.SelectAccessors()).Each(a =>
                {
                    alterProjection = p => p.OuterJoin(a);
                });
            }


            _projectionBuilders.Each(x => x(projection));

            return new ProjectionDataSource<TEntity>(projection, SelectMode);
        }

        protected Projection<TEntity>.WhereExpression Where(Expression<Func<TEntity, object>> property)
        {
            var list = new List<ICriterion>();
            var expression = new Projection<TEntity>.WhereExpression(property, list);
            alterProjection = projection => list.Each(projection.AddRestriction);

            return expression;
        }


    }
}