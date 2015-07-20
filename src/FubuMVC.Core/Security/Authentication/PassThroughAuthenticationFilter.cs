using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Security.Authentication
{
	public class PassThroughAuthenticationFilter
	{
		private readonly IAuthenticationService _authentication;
		private readonly ICurrentChain _currentChain;

		public PassThroughAuthenticationFilter(IAuthenticationService authentication, ICurrentChain currentChain)
		{
			_authentication = authentication;
			_currentChain = currentChain;
		}

		public FubuContinuation Filter()
		{
			if (!_currentChain.IsInPartial())
			{
				_authentication.TryToApply();
			}

			return FubuContinuation.NextBehavior();
		}
	}
}