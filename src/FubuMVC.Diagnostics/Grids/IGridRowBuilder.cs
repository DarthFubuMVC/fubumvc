using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public interface IGridRowBuilder<TModel, TRow>
    {
        IEnumerable<JsonGridRow> RowsFor(TModel target, IEnumerable<JsonGridFilter> filters);
    }
}