using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Infrastructure.Grids;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Endpoints.Routes
{
    public class FilterEndpoint
    {
        private readonly BehaviorGraph _graph;
        private readonly IGridService<BehaviorGraph> _gridService;

        public FilterEndpoint(BehaviorGraph graph, IGridService<BehaviorGraph> gridService)
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