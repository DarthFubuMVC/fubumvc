using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public interface IGridColumnBuilder<T>
    {
        IEnumerable<JsonGridColumn> ColumnsFor(T target);
    }
}