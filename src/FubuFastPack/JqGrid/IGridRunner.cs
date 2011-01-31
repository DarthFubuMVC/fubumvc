using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridRunner<TEntity, TService>
    {
        TService Service { get; }

        GridResults RunGrid<T>(GridDefinition<T> grid, IGridDataSource<T> source, PagingOptions request);
    }
}