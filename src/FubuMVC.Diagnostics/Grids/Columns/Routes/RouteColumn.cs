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

		public override int Rank()
		{
			return 5;
		}

		public override string ValueFor(BehaviorChain chain)
		{
            if (chain.Route == null) return " -";

            var pattern = chain.Route.Pattern;
            if (pattern == string.Empty)
            {
                pattern = "(default)";
            }

            return pattern;
		}
	}
}