using FubuMVC.Core.Ajax;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Tests.Security.Authentication
{
    [NotAuthenticated]
    public class NotAuthenticatedModel
    {
    }

    public class AuthenticatedModel
    {
        
    }

	[PassThroughAuthentication]
	public class PassThroughModel
	{
		
	}

    [NotAuthenticated]
    public class NotAuthenticatedEndpoint
    {
        public AjaxContinuation get_something()
        {
            return AjaxContinuation.Successful();
        }
    }

    public class TestAuthenticationEndpoint<T>
    {
        public AjaxContinuation get_something(T input)
        {
            return AjaxContinuation.Successful();
        }
    }
}