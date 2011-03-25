using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Endpoints.Routes
{
    public class FilterEndpoint
    {
        private readonly BehaviorGraph _graph;
        private readonly IGridService<BehaviorGraph, BehaviorChain> _gridService;

        public FilterEndpoint(BehaviorGraph graph, IGridService<BehaviorGraph, BehaviorChain> gridService)
        {
            _graph = graph;
            _gridService = gridService;
        }

        public JsonGridModel Post(RouteQuery query)
        {
        	var gridQuery = JsonService.Deserialize<JsonGridQuery>(query.Body);
			return _gridService.GridFor(_graph, gridQuery);
        }
    }
}