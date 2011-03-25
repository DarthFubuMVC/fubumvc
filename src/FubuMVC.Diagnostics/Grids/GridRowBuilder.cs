using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public class GridRowBuilder<TModel, TRow> : IGridRowBuilder<TModel, TRow>
    {
        private readonly IEnumerable<IGridColumnBuilder<TRow>> _columnBuilders;
        private readonly IEnumerable<IGridFilter<TRow>> _gridFilters;
        private readonly IGridRowProvider<TModel, TRow> _rowProvider;

        public GridRowBuilder(IEnumerable<IGridColumnBuilder<TRow>> columnBuilders, IEnumerable<IGridFilter<TRow>> gridFilters, IGridRowProvider<TModel, TRow> rowProvider)
        {
            _columnBuilders = columnBuilders;
            _rowProvider = rowProvider;
            _gridFilters = gridFilters;
        }

        public IEnumerable<JsonGridRow> RowsFor(TModel target, IEnumerable<JsonGridFilter> filters)
        {
            return _rowProvider
                .RowsFor(target)
                .Where(chain => _gridFilters.Matches(filters, chain))
                .Select(chain =>
                {
                    var columns = _columnBuilders
                        .SelectMany(builder => builder.ColumnsFor(chain))
                        .ToList();

                    var idCol = columns.FirstOrDefault(col => col.IsIdentifier);
                    if (idCol == null)
                    {
                        throw new DiagnosticsException(1001,
                                                       "No Identifier Column specified for {0}.", typeof(TRow).Name);
                    }

                    return new JsonGridRow
                    {
                        Id = idCol.Value,
                        Columns = columns
                    };
                });
        }
    }
}