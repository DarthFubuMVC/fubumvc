using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class InputModelFilter : GridFilterBase<InputModelColumn, BehaviorChain>
    {
        public InputModelFilter(InputModelColumn column)
			: base(column)
        {
        }
    }
}