using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class RouteColumn : BehaviorChainColumnBase
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

		public override bool IsIdentifier()
		{
			return false;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return false;
		}

		public override bool HideFilter(BehaviorChain chain)
		{
			return false;
		}
	}
}