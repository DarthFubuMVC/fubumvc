using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class OutputModelFilter : GridFilterBase<OutputModelColumn, BehaviorChain>
	{
		public OutputModelFilter(OutputModelColumn column)
			: base(column)
		{
		}
	}
}