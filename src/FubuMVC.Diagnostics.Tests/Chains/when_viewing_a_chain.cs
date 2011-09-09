using System;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Features.Chains;
using FubuMVC.Diagnostics.Features.Chains.View;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Chains
{
	[TestFixture]
	public class when_viewing_a_chain : InteractionContext<get_Id_handler>
	{
		private BehaviorGraph _graph;
		protected override void beforeEach()
		{
			_graph = ObjectMother.DiagnosticsGraph();
			Container.Configure(x => x.For<BehaviorGraph>().Use(_graph));
		}

		[Test]
		public void should_throw_argument_exception_if_chain_cannot_be_found()
		{
			var request = new ChainRequest {Id = Guid.NewGuid()};

			Exception<ArgumentException>
				.ShouldBeThrownBy(() => ClassUnderTest.Execute(request));
		}

		[Test]
		public void should_set_chain_on_model_when_found()
		{
			var chain = _graph.BehaviorFor(typeof(DashboardRequestModel));
			var request = new ChainRequest {Id = chain.UniqueId};

			ClassUnderTest
				.Execute(request)
				.Chain
				.ShouldEqual(chain);
		}
	}
}