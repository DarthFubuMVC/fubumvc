using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class RouteColumn : GridColumnBase<BehaviorChain>
	{
		public RouteColumn()
			: base(chain => chain.Route)
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			var route = chain.RoutePattern;
			if (chain.Route == null)
			{
				route = "(no route)";
			}
			else if (chain.RoutePattern.IsEmpty())
			{
				route = "(default)";
			}

			return route;
		}
	}
}