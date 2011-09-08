using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Core.Grids.Filters.Routes
{
	public class AuthorizationFilter : GridFilterBase<AuthorizationColumn, BehaviorChain>
    {
    	public AuthorizationFilter(AuthorizationColumn column) 
			: base(column)
    	{
    	}
    }
}