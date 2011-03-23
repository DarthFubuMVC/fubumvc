using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public class GridService<T> : IGridService<T>
    {
        private readonly IGridRowBuilder<T> _gridRowBuilder;

        public GridService(IGridRowBuilder<T> gridRowBuilder)
        {
            _gridRowBuilder = gridRowBuilder;
        }

        public JsonGridModel GridFor(T target, JsonGridQuery query)
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