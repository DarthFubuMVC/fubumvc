using FubuFastPack.Domain;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridRunner<TEntity, out TService> where TEntity : DomainEntity
    {
        TService Service { get; }

        GridResults RunGrid(GridDefinition<TEntity> grid, IGridDataSource<TEntity> source, GridDataRequest request);
        int GetCount(IGridDataSource<TEntity> source);
    }
}