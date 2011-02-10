using System;
using FubuFastPack.Domain;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface IGridDataSource<T> where T : DomainEntity
    {
        int TotalCount();
        IGridData Fetch(GridDataRequest options);

        void ApplyRestrictions(Action<IDataSourceFilter<T>> configure);
        void ApplyCriteria(FilterRequest<T> request, IQueryService queryService);
    }
}