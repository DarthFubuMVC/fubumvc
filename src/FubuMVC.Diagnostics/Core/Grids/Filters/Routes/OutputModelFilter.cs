using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class OutputModelFilter : GridFilterBase<OutputModelColumn, BehaviorChain>
	{
		public OutputModelFilter(OutputModelColumn column)
			: base(column)
		{
		}
	}
}