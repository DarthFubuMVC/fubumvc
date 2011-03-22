using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public static class GridExtensions
    {
        public static IEnumerable<JsonGridRow> OrderBy(this IEnumerable<JsonGridRow> rows, string columnName, string sortOrder)
        {
            var orderedRows = new List<JsonGridRow>(rows);
            orderedRows.Sort((x, y) => RowComparer.CompareRows(columnName, x, y));

            if (!sortOrder.IsEmpty() && sortOrder.Equals(JsonGridQuery.DESCENDING, StringComparison.InvariantCultureIgnoreCase))
            {
                orderedRows.Reverse();
            }

            return orderedRows;
        }

        public static IEnumerable<JsonGridRow> Page(this IEnumerable<JsonGridRow> rows, int itemsPerPage, int pageNr, out int totalRecords, out int totalPages)
        {
            totalRecords = rows.Count();
            totalPages = 1;

            if(itemsPerPage == -1)
            {
                return rows;
            }

            if (totalRecords != 0)
            {
                totalPages = (int)Math.Ceiling((double)totalRecords / (itemsPerPage == 0 ? 20 : itemsPerPage));
            }

            if(pageNr != 0)
            {
                --pageNr;
            }

            return rows
                .Skip(itemsPerPage * pageNr)
                .Take(itemsPerPage);
        }

        public static IEnumerable<JsonGridRow> ApplyFilters(this IEnumerable<JsonGridRow> rows, IEnumerable<JsonGridFilter> filters)
        {
            filters.Each(filter => rows = rows.ApplyFilter(filter));
            return rows;
        }

        public static IEnumerable<JsonGridRow> ApplyFilter(this IEnumerable<JsonGridRow> rows, JsonGridFilter filter)
        {
            var filteredRows = new List<JsonGridRow>();
            rows
                .Each(row =>
                          {
                              if(!row.Columns.Any(c =>c.Name.ToLower().Equals(filter.ColumnName.ToLower())))
                              {
                                  return;
                              }

                              // TODO: We should do some better matching here
                              if(row.Columns.Any(c => filter.Values.Any(value => c.Value.ToLower().Contains(value.ToLower()))))
                              {
                                  filteredRows.Add(row);
                              }
                          });

            return filteredRows;
        }
    }
}