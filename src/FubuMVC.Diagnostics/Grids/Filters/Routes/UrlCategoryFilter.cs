using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class UrlCategoryFilter : GridFilterBase<UrlCategoryColumn, BehaviorChain>
    {
    	public UrlCategoryFilter(UrlCategoryColumn column) 
			: base(column)
    	{
    	}
    }
}