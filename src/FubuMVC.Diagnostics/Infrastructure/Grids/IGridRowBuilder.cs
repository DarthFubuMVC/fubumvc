using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public interface IGridRowBuilder<T>
    {
        IEnumerable<JsonGridRow> RowsFor(T target);
    }
}