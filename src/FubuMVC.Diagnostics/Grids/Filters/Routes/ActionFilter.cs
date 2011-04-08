using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class ActionFilter : GridFilterBase<ActionColumn, BehaviorChain>
	{
		public ActionFilter(ActionColumn column)
			: base(column)
		{
		}
	}
}