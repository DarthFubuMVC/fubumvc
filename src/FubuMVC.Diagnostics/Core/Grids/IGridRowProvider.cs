using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Core.Grids
{
    public interface IGridRowProvider<TModel, TRow>
    {
        IEnumerable<TRow> RowsFor(TModel target);
    }
}