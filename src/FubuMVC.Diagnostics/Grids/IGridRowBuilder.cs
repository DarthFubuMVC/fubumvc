using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public interface IGridRowBuilder<T>
    {
        IEnumerable<JsonGridRow> RowsFor(T target, IEnumerable<JsonGridFilter> filters);
    }
}