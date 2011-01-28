using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridDataSource
    {
        int TotalCount();
        IGridData Fetch(PagingOptions options);
    }
}