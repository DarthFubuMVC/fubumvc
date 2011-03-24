using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
    public class UrlCategoryFilter : BehaviorChainFilterBase<UrlCategoryColumn>
    {
    	public UrlCategoryFilter(UrlCategoryColumn column) 
			: base(column)
    	{
    	}
    }
}