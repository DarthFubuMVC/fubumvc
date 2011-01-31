using System;
using System.Collections.Generic;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;

namespace FubuFastPack.JqGrid
{
    public class ProjectionGrid<TEntity> : Grid<TEntity, Projection<TEntity>> where TEntity : DomainEntity
    {
        private readonly IList<Action<Projection<TEntity>>> _projectionBuilders = new List<Action<Projection<TEntity>>>();


        private Action<Projection<TEntity>> alterProjection
        {
            set { _projectionBuilders.Add(value); }
        }

        public override IGridDataSource<TEntity> BuildSource(Projection<TEntity> projection)
        {
            Definition.SelectedAccessors.Each(a => projection.AddColumn(a));
            _projectionBuilders.Each(x => x(projection));

            return new ProjectionDataSource<TEntity>(projection);
        }
    }
}