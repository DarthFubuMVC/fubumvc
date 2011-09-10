using System;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Features.Chains;
using FubuMVC.Diagnostics.Features.Chains.View;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Chains
{
	[TestFixture]
	public class when_viewing_a_chain : InteractionContext<get_Id_handler>
	{
		private BehaviorGraph _graph;
	    private ChainRequest _request;
	    private BehaviorChain _chain;

		protected override void beforeEach()
		{
			_graph = ObjectMother.DiagnosticsGraph();
            _chain = _graph.BehaviorFor(typeof(DashboardRequestModel));
            _request = new ChainRequest { Id = _chain.UniqueId };

			Container.Configure(x => x.For<BehaviorGraph>().Use(_graph));
		}

		[Test]
		public void should_throw_argument_exception_if_chain_cannot_be_found()
		{
			Exception<ArgumentException>
                .ShouldBeThrownBy(() => ClassUnderTest.Execute(new ChainRequest { Id = Guid.NewGuid() }));
		}

		[Test]
		public void should_set_chain_on_model_when_found()
		{
			ClassUnderTest
				.Execute(_request)
				.Chain
				.ShouldEqual(_chain);
		}

        [Test]
        public void should_resolve_contraints()
        {
            var constraints = "POST";
            MockFor<IHttpConstraintResolver>()
                .Expect(r => r.Resolve(_chain))
                .Return(constraints);

            ClassUnderTest
                .Execute(_request)
                .Constraints
                .ShouldEqual(constraints);
        }

        [Test]
        public void should_build_behaviors()
        {
            ClassUnderTest
                .Execute(_request)
                .Behaviors
                .ShouldHaveCount(_chain.Count());
        }

        [Test]
        public void should_build_action_call_logs()
        {
            var call = _chain.FirstCall();
            var logs = _graph.Observer.GetLog(call);

            ClassUnderTest
                .Execute(_request)
                .Behaviors
                .Single(b => b.BehaviorType == call.ToString())
                .Logs
                .ShouldEqual(logs);
        }
	}
}