using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Features.Chains.View;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Chains
{
    [TestFixture]
    public class when_building_a_chain_visualizer : InteractionContext<ChainVisualizerBuilder>
    {
        private BehaviorGraph _graph;
        private BehaviorChain _chain;

        protected override void beforeEach()
        {
            _graph = ObjectMother.DiagnosticsGraph();
            _chain = _graph.BehaviorFor(typeof(DashboardRequestModel));

            Container.Configure(x => x.For<BehaviorGraph>().Use(_graph));
        }

        [Test]
        public void should_set_chain_on_model_when_found()
        {
            ClassUnderTest
                .VisualizerFor(_chain.UniqueId)
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
                .VisualizerFor(_chain.UniqueId)
                .Constraints
                .ShouldEqual(constraints);
        }

        [Test]
        public void should_build_behaviors()
        {
            ClassUnderTest
                .VisualizerFor(_chain.UniqueId)
                .Behaviors
                .ShouldHaveCount(_chain.Count());
        }

        [Test]
        public void should_build_action_call_logs()
        {
            var call = _chain.FirstCall();
            var logs = _graph.Observer.GetLog(call);

            ClassUnderTest
                .VisualizerFor(_chain.UniqueId)
                .Behaviors
                .Single(b => b.BehaviorType == call.ToString())
                .Logs
                .ShouldEqual(logs);
        }
    }
}