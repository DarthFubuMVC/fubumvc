using System;
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
            var data = _dataBuilder.BuildRoutes(_graph);
            var totalRecords = data.Count();
            var totalPages = 0;
            var nrRows = query.rows == 0 ? 20 : query.rows;

            if(totalRecords != 0)
            {
                totalPages = (int)Math.Ceiling((double)totalRecords / nrRows);
            }

            var rows = data
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