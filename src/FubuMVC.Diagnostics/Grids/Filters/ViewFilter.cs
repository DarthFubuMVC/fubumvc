using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
    public class ViewFilter : BehaviorChainFilterBase<ViewColumn>
    {
    	public ViewFilter(ViewColumn column) 
			: base(column)
    	{
    	}
    }
}