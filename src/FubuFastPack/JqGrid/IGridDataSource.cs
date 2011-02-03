using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridDataSource<T>
    {
        int TotalCount();
        IGridData Fetch(GridDataRequest options);

        void ApplyCriteria(FilterRequest<T> request, IQueryService queryService);
    }
}