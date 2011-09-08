using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class UrlCategoryFilter : GridFilterBase<UrlCategoryColumn, BehaviorChain>
    {
    	public UrlCategoryFilter(UrlCategoryColumn column) 
			: base(column)
    	{
    	}
    }
}