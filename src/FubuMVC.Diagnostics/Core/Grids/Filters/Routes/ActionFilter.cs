using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class ActionFilter : GridFilterBase<ActionColumn, BehaviorChain>
	{
		public ActionFilter(ActionColumn column)
			: base(column)
		{
		}
	}
}