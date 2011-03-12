using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Endpoints.Routes
{
    public class FilterEndpoint
    {
        private readonly BehaviorGraph _graph;
        private readonly IRouteDataBuilder _dataBuilder;

        public FilterEndpoint(BehaviorGraph graph, IRouteDataBuilder dataBuilder)
        {
            _graph = graph;
            _dataBuilder = dataBuilder;
        }

        public JsonGridModel Get(RouteQuery query)
        {
            var data = _dataBuilder.BuildRoutes(_graph).AsQueryable();
            var totalRecords = data.Count();
            var totalPages = 0;
            if(totalRecords != 0)
            {
                totalPages = totalRecords / query.rows;
            }

            var page = query.page;
            if(page >= 1)
            {
                page = page - 1;
            }

            IEnumerable<RouteDataModel> orderedRows;
            if(query.sord == "asc")
            {
                orderedRows = data.OrderBy(query.sidx);
            }
            else
            {
                orderedRows = data.OrderByDescending(query.sidx);
            }
                
            var rows = orderedRows
                        .Skip(page*query.rows)
                        .Take(query.rows)
                        .Select(d => new JsonGridRow
                                         {
                                             id = d.Id,
                                             cell = new[] { d.Route, d.Constraints, d.Action, d.InputModel, d.OutputModel }
                                         });

            return new JsonGridModel
                       {
                           page = query.page,
                           records = totalRecords,
                           total = totalPages,
                           rows = rows.ToList()
                       };
        }

        
    }
}