using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
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
        GridColumnDTO ToDto();
        Action<EntityDTO> CreateFiller(IGridData data, IDisplayFormatter formatter, IUrlRegistry urls);

        IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService);
        IEnumerable<Accessor> SelectAccessors();


        // Prolly shouldn't be part of the interface
        ColumnFetching FetchMode { get; set; }

        
        
        // Not wild about these being public
        bool IsFilterable { get; set; }
        bool IsSortable { get; set; }

    }


}