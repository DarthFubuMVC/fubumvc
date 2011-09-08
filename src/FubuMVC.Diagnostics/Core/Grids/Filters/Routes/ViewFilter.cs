using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class ViewFilter : GridFilterBase<ViewColumn, BehaviorChain>
    {
    	public ViewFilter(ViewColumn column) 
			: base(column)
    	{
    	}
    }
}