using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
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
        ColumnFetching FetchMode { get; set; }
        bool IsFilterable { get; set; }
        bool IsSortable { get; set; }
        Accessor Accessor { get; }
        Expression Expression { get; }
    }

    public static class GridColumnExtensions
    {
        public static Expression<Func<T, object>> PropertyExpressionFor<T>(this IGridColumn column)
        {
            return (Expression<Func<T, object>>) column.Expression;
        }

        
    }


}