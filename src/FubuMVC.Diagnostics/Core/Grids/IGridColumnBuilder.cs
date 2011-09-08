using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Core.Grids
{
    public interface IGridColumnBuilder<T>
    {
        IEnumerable<JsonGridColumn> ColumnsFor(T target);
    }
}