using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class OriginFilter : GridFilterBase<OriginColumn, BehaviorChain>
    {
    	public OriginFilter(OriginColumn column) 
			: base(column)
    	{
    	}
    }
}