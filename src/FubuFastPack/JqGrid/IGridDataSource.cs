using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridDataSource<T>
    {
        int TotalCount();
        IGridData Fetch(PagingOptions options);

        void ApplyCriteria(FilterRequest<T> request, IFilterHandler handler);
    }
}