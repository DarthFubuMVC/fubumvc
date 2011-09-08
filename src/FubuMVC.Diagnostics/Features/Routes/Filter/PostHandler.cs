using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Routes.Filter
{
    public class PostHandler
    {
        private readonly BehaviorGraph _graph;
        private readonly IGridService<BehaviorGraph, BehaviorChain> _gridService;

        public PostHandler(BehaviorGraph graph, IGridService<BehaviorGraph, BehaviorChain> gridService)
        {
            _graph = graph;
            _gridService = gridService;
        }

        public JsonGridModel Execute(JsonGridQuery<BehaviorGraph> query)
        {
			return _gridService.GridFor(_graph, query);
        }
    }
}