using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class OriginFilter : GridFilterBase<OriginColumn, BehaviorChain>
    {
    	public OriginFilter(OriginColumn column) 
			: base(column)
    	{
    	}
    }
}