using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
	// TODO -- Change UrlRegistry and EndpointService to use this
	// TODO -- Change the NavigationService in FubuMVC.Navigation to use this
	// TODO -- This is going to break Serenity

	public interface IChainUrlResolver
	{
		string UrlFor(object model, BehaviorChain chain);
		string UrlFor(BehaviorChain chain, RouteParameters parameters);
	}

	public class ChainUrlResolver : IChainUrlResolver
	{
		private readonly ICurrentHttpRequest _httpRequest;

		public ChainUrlResolver(ICurrentHttpRequest httpRequest)
		{
			_httpRequest = httpRequest;
		}

		public string UrlFor(object model, BehaviorChain chain)
		{
			var url = chain.Route.CreateUrlFromInput(model);
			return _httpRequest.ToFullUrl(url);
		}

		public string UrlFor(BehaviorChain chain, RouteParameters parameters)
		{
			var url = chain.Route.Input.CreateUrlFromParameters(parameters);

			return _httpRequest.ToFullUrl(url);
		}
	}
}