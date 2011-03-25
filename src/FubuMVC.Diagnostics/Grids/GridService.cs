using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public class GridService<TModel, TRow> : IGridService<TModel, TRow>
    {
        private readonly IGridRowBuilder<TModel, TRow> _gridRowBuilder;

        public GridService(IGridRowBuilder<TModel, TRow> gridRowBuilder)
        {
            _gridRowBuilder = gridRowBuilder;
        }

        public JsonGridModel GridFor(TModel target, JsonGridQuery query)
        {
            int totalRecords;
            int totalPages;
            var rows = _gridRowBuilder
                            .RowsFor(target, query.Filters)
                            .OrderBy(query.sidx, query.sord)
                            .Page(query.rows == 0 ? 20 : query.rows, query.page, out totalRecords, out totalPages);

            return new JsonGridModel
                       {
                           PageNr = query.page,
                           TotalRecords = totalRecords,
                           TotalPages = totalPages,
                           Rows = rows
                       };
        }
    }
}