using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
    public class OriginFilter : BehaviorChainFilterBase<OriginColumn>
    {
    	public OriginFilter(OriginColumn column) 
			: base(column)
    	{
    	}
    }
}