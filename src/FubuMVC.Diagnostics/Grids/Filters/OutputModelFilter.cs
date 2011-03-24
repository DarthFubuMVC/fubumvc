using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public class OutputModelFilter : BehaviorChainFilterBase<OutputModelColumn>
	{
		public OutputModelFilter(OutputModelColumn column)
			: base(column)
		{
		}
	}
}