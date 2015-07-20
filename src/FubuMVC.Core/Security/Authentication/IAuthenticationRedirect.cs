using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
	public interface IAuthenticationRedirect
	{
		bool Applies();
		FubuContinuation Redirect();
	}
}