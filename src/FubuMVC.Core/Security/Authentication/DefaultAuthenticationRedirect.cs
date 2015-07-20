using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Security.Authentication
{
	public class DefaultAuthenticationRedirect : IAuthenticationRedirect
	{
		private readonly IHttpRequest _currentRequest;

		public DefaultAuthenticationRedirect(IHttpRequest currentRequest)
		{
			_currentRequest = currentRequest;
		}

		public bool Applies()
		{
			return true;
		}

		public FubuContinuation Redirect()
		{
			return FubuContinuation.RedirectTo(new LoginRequest
			{
				Url = _currentRequest.RelativeUrl()
			}, "GET");
		}
	}
}