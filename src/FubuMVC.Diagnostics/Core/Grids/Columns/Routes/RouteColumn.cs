using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
    [MarkedForTermination]
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
            if (chain.Route == null || chain.Route.Pattern == null)
            {
                return "N/A";
            }

            if(chain.IsPartialOnly)
            {
                return "(partial)";
            }

            var pattern = chain.Route.Pattern;
            if (pattern == string.Empty)
            {
                pattern = "(default)";
            }

            return pattern;
		}
	}
}