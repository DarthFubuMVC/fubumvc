using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns.Routes;

namespace FubuMVC.Diagnostics.Grids.Filters.Routes
{
	public class AuthorizationFilter : GridFilterBase<AuthorizationColumn, BehaviorChain>
    {
    	public AuthorizationFilter(AuthorizationColumn column) 
			: base(column)
    	{
    	}
    }
}