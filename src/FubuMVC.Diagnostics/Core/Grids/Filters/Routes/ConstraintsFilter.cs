using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class ConstraintsFilter : GridFilterBase<ConstraintsColumn, BehaviorChain>
	{
		public ConstraintsFilter(ConstraintsColumn column)
			: base(column)
		{
		}
	}
}