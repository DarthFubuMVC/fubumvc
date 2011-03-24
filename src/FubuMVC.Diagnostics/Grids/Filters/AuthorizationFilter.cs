using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
    public class AuthorizationFilter : BehaviorChainFilterBase<AuthorizationColumn>
    {
    	public AuthorizationFilter(AuthorizationColumn column) 
			: base(column)
    	{
    	}
    }
}