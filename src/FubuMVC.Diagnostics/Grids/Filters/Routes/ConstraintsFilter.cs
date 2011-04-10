using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class ConstraintsFilter : GridFilterBase<ConstraintsColumn, BehaviorChain>
	{
		public ConstraintsFilter(ConstraintsColumn column)
			: base(column)
		{
		}
	}
}