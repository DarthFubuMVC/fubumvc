using System;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Urls
{
	
	public class ChainUrlResolverTester : IDisposable
	{
		private OwinHttpRequest theHttpRequest;
		private ChainUrlResolver theUrlResolver;

		private BehaviorGraph theGraph;
		private BehaviorChain theSimpleChain;
		private BehaviorChain theChainWithRouteParams;

	    public ChainUrlResolverTester()
	    {
            theHttpRequest = OwinHttpRequest.ForTesting();
            UrlContext.Stub("http://server");

            theUrlResolver = new ChainUrlResolver(theHttpRequest);

            theGraph = BehaviorGraph.BuildFrom(registry =>
            {
                registry.Actions.IncludeType<ChainUrlResolverEndpoint>();
            });

            theSimpleChain = theGraph.ChainFor<ChainUrlResolverEndpoint>(x => x.get_index());
            theChainWithRouteParams = theGraph.ChainFor(typeof(ChainUrlParams));
        }


		public void Dispose()
		{
			UrlContext.Reset();
		}

		[Fact]
		public void resolve_a_url_without_route_params()
		{
			theUrlResolver.UrlFor(null, theSimpleChain).ShouldBe("/index");
		}

		[Fact]
		public void resolve_a_url_with_route_params_filled_by_input()
		{
			var input = new ChainUrlParams {Name = "Joel"};
			theUrlResolver.UrlFor(input, theChainWithRouteParams).ShouldBe("/Joel");
		}

		[Fact]
		public void resolve_a_url_with_route_params_explicitly_filled()
		{
			var values = new RouteParameters();
			values["Name"] = "Joel";

			theUrlResolver.UrlFor(theChainWithRouteParams, values).ShouldBe("/Joel");
		}

		public class ChainUrlResolverEndpoint
		{
			public string get_Name(ChainUrlParams values)
			{
				throw new NotImplementedException();
			}

			public string get_index()
			{
				throw new NotImplementedException();
			}
		}

		public class ChainUrlParams
		{
			public string Name { get; set; }
		}
	}
}