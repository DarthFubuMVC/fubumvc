using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
	public interface IAuthenticationRedirect
	{
		bool Applies();
		FubuContinuation Redirect();
	}
}