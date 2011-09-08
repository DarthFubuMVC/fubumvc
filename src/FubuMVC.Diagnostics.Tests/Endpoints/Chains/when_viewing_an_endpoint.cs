using System;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Core;
using FubuMVC.Diagnostics.Endpoints.Chains;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Models;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Endpoints.Chains
{
	[TestFixture]
	public class when_viewing_an_endpoint : InteractionContext<ViewEndpoint>
	{
		private BehaviorGraph _graph;
		protected override void beforeEach()
		{
			_graph = ObjectMother.DiagnosticsGraph();
			Container.Configure(x => x.For<BehaviorGraph>().Use(_graph));
		}

		[Test]
		public void should_throw_unknown_chain_exception_if_chain_cannot_be_found()
		{
			var request = new ChainRequest {Id = Guid.NewGuid()};

			Exception<UnknownObjectException>
				.ShouldBeThrownBy(() => ClassUnderTest.Get(request));
		}

		[Test]
		public void should_set_chain_on_model_when_found()
		{
			var chain = _graph.BehaviorFor(typeof(DashboardRequestModel));
			var request = new ChainRequest {Id = chain.UniqueId};

			ClassUnderTest
				.Get(request)
				.Chain
				.ShouldEqual(chain);
		}
	}
}