using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public class ConstraintsFilter : BehaviorChainFilterBase<ConstraintsColumn>
	{
		public ConstraintsFilter(ConstraintsColumn column)
			: base(column)
		{
		}
	}
}