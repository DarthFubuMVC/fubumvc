using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public enum ColumnFetching
    {
        FetchAndDisplay,
        FetchOnly,
        NoFetch
    }


    public interface IGridColumn
    {
        IDictionary<string, object> ToDictionary();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);

        IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService);
        IEnumerable<Accessor> SelectAccessors();
    }
}