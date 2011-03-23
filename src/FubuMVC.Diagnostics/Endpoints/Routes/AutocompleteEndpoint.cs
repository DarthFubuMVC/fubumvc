using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Infrastructure.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Endpoints.Routes
{
    public class AutocompleteEndpoint
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly IGridService<BehaviorGraph> _gridService;

        public AutocompleteEndpoint(BehaviorGraph behaviorGraph, IGridService<BehaviorGraph> gridService)
        {
            _behaviorGraph = behaviorGraph;
            _gridService = gridService;
        }

        public JsonAutocompleteResultModel Post(AutocompleteRequestModel request)
        {
            var filter = new JsonGridFilter {ColumnName = request.Column, Values = new List<string> {request.Query}};
            var query = new JsonGridQuery
                            {
                                Filters = new List<JsonGridFilter> {filter},
                                page = 0,
                                rows = -1,
                                sidx = request.Column,
                                sord = JsonGridQuery.ASCENDING
                            };
            return new JsonAutocompleteResultModel
                       {
                           Values = _gridService
                               .GridFor(_behaviorGraph, query)
                               .Rows
                               .SelectMany(r => r.Columns.Where(c => c.Name.Equals(request.Column, StringComparison.OrdinalIgnoreCase)))
                               .Distinct()
                       };
        }
    }
}