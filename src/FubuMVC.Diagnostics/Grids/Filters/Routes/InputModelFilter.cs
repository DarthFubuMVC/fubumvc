using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class InputModelFilter : GridFilterBase<InputModelColumn, BehaviorChain>
    {
        public InputModelFilter(InputModelColumn column)
			: base(column)
        {
        }
    }
}