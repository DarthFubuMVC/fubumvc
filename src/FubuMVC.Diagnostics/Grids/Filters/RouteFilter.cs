using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public class RouteFilter : BehaviorChainFilterBase<RouteColumn>
    {
    	public RouteFilter(RouteColumn column)
			: base(column)
    	{
    	}

        public override bool Matches(BehaviorChain target, JsonGridFilter filter)
        {
        	return startsWith(target, filter);
        }
    }
}