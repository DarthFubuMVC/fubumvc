using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public class ActionFilter : BehaviorChainFilterBase<ActionColumn>
	{
		public ActionFilter(ActionColumn column)
			: base(column)
		{
		}
	}
}